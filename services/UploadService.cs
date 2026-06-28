using AutoMapper;
using ClosedXML.Excel;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface IUploadService
{
    Task<UploadResultDto> UploadVendorExcelAsync(Stream fileStream, string fileName, int uploadedByUserId);
    Task<UploadResultDto> UploadProductExcelAsync(Stream fileStream, string fileName, int uploadedByUserId);
    Task<UploadResultDto> UploadSupplyOrderExcelAsync(Stream fileStream, string fileName, int uploadedByUserId);
    Task<PagedResult<UploadHistoryResponseDto>> GetHistoryAsync(UploadHistoryFilterParams filter);
}

public class UploadService : IUploadService
{
    private readonly IUploadHistoryRepository _uploadHistoryRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IProductRepository _productRepository;
    private readonly ISupplyOrderRepository _supplyOrderRepository;
    private readonly IMapper _mapper;

    public UploadService(
        IUploadHistoryRepository uploadHistoryRepository,
        IVendorRepository vendorRepository,
        IProductRepository productRepository,
        ISupplyOrderRepository supplyOrderRepository,
        IMapper mapper)
    {
        _uploadHistoryRepository = uploadHistoryRepository;
        _vendorRepository = vendorRepository;
        _productRepository = productRepository;
        _supplyOrderRepository = supplyOrderRepository;
        _mapper = mapper;
    }

    // Expected columns, in order, row 1 = header (skipped):
    // URN_No | CIN_No | MSME_No | GST_No | PAN_No | Vendor_Org_Name |
    // Vendor_Org_Email | Nodal_Officer_Name | Nodal_Officer_Email | Nodal_Officer_Mobile |
    // Nodal_Officer_Designation | Vendor_Code_assigned_by_MDL
    public async Task<UploadResultDto> UploadVendorExcelAsync(Stream fileStream, string fileName, int uploadedByUserId)
    {
        var rowErrors = new List<string>();
        var successCount = 0;
        var totalCount = 0;

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RowsUsed().Skip(1); // skip header row

        foreach (var row in rows)
        {
            totalCount++;
            try
            {
                var urn = row.Cell(1).GetString().Trim();
                var orgName = row.Cell(6).GetString().Trim();

                if (string.IsNullOrWhiteSpace(urn))
                {
                    rowErrors.Add($"Row {row.RowNumber()}: URN_No is required.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(orgName))
                {
                    rowErrors.Add($"Row {row.RowNumber()}: Vendor_Org_Name is required.");
                    continue;
                }

                // URN_No is the PK, so "existing" lookup is just a find-by-id.
                var existing = await _vendorRepository.GetByURNNoAsync(urn);

                var vendor = existing ?? new Vendor { URN_No = urn };
                vendor.CIN_No = row.Cell(2).GetString().Trim();
                vendor.MSME_No = row.Cell(3).GetString().Trim();
                vendor.GST_No = row.Cell(4).GetString().Trim();
                vendor.PAN_No = row.Cell(5).GetString().Trim();
                vendor.Vendor_Org_Name = orgName;
                vendor.Vendor_Org_Email = row.Cell(7).GetString().Trim();
                vendor.Nodal_Officer_Name = row.Cell(8).GetString().Trim();
                vendor.Nodal_Officer_Email = row.Cell(9).GetString().Trim();
                vendor.Nodal_Officer_Mobile = row.Cell(10).GetString().Trim();
                vendor.Nodal_Officer_Designation = row.Cell(11).GetString().Trim();
                vendor.Vendor_Code_assigned_by_MDL = row.Cell(12).GetString().Trim();
                vendor.IsActive = true;
                vendor.Manual_Bulk_upload = true;

                if (existing is null)
                {
                    await _vendorRepository.AddAsync(vendor);
                }
                else
                {
                    _vendorRepository.Update(vendor);
                }

                successCount++;
            }
            catch (Exception ex)
            {
                rowErrors.Add($"Row {row.RowNumber()}: {ex.Message}");
            }
        }

        await _vendorRepository.SaveChangesAsync();

        return await RecordUploadHistoryAsync(
            SyncEntityType.Vendor, fileName, totalCount, successCount, rowErrors, uploadedByUserId);
    }

    // Expected columns: URN_No | Product_Name | Defence_Platform | Product_Type |
    // Part_No | Is_Item_Supplied | SO_No | SO_Date
    public async Task<UploadResultDto> UploadProductExcelAsync(Stream fileStream, string fileName, int uploadedByUserId)
    {
        var rowErrors = new List<string>();
        var successCount = 0;
        var totalCount = 0;

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            totalCount++;
            try
            {
                var urn = row.Cell(1).GetString().Trim();
                var productName = row.Cell(2).GetString().Trim();

                if (string.IsNullOrWhiteSpace(urn) || string.IsNullOrWhiteSpace(productName))
                {
                    rowErrors.Add($"Row {row.RowNumber()}: URN_No and Product_Name are required.");
                    continue;
                }

                var vendorExists = await _vendorRepository.ExistsAsync(v => v.URN_No == urn);
                if (!vendorExists)
                {
                    rowErrors.Add($"Row {row.RowNumber()}: Vendor with URN_No '{urn}' does not exist.");
                    continue;
                }

                // Note: GetByUrnAsync here matches on Product.URN_No (the FK), which is NOT unique
                // per product the way it was on Vendor — multiple products can share a vendor's URN_No.
                // If you need an upsert-by-natural-key for products, tell me what that key should be
                // (e.g. URN_No + Product_Name + SO_No) and I'll adjust; for now every row is a fresh insert.
                var product = new Product
                {
                    URN_No = urn,
                    Product_Name = productName,
                    Defence_Platform = row.Cell(3).GetString().Trim(),
                    Product_Type = row.Cell(4).GetString().Trim(),
                    Part_No = row.Cell(5).GetString().Trim(),
                    Is_Item_Supplied = row.Cell(6).GetString().Trim().Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                       row.Cell(6).GetString().Trim() == "1",
                    SO_No = row.Cell(7).GetString().Trim(),
                    SO_Date = row.Cell(8).TryGetValue<DateTime>(out var soDate) ? soDate : null,
                    IsActive = true,
                    Manual_Bulk_upload = true
                };

                await _productRepository.AddAsync(product);
                successCount++;
            }
            catch (Exception ex)
            {
                rowErrors.Add($"Row {row.RowNumber()}: {ex.Message}");
            }
        }

        await _productRepository.SaveChangesAsync();

        return await RecordUploadHistoryAsync(
            SyncEntityType.Product, fileName, totalCount, successCount, rowErrors, uploadedByUserId);
    }

    // Expected columns: ProductId | PRO_No | PO_No | PO_Date | URN_No |
    // Whether_MSME | Qty_Ordered | Unit_Rate | Qty_Supplied
    public async Task<UploadResultDto> UploadSupplyOrderExcelAsync(Stream fileStream, string fileName, int uploadedByUserId)
    {
        var rowErrors = new List<string>();
        var successCount = 0;
        var totalCount = 0;

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();
        var rows = worksheet.RowsUsed().Skip(1);

        foreach (var row in rows)
        {
            totalCount++;
            try
            {
                var productId = row.Cell(1).GetValue<int>();
                var poNumber = row.Cell(3).GetString().Trim();
                var urn = row.Cell(5).GetString().Trim();

                if (productId <= 0 || string.IsNullOrWhiteSpace(poNumber) || string.IsNullOrWhiteSpace(urn))
                {
                    rowErrors.Add($"Row {row.RowNumber()}: ProductId, PO_No and URN_No are required.");
                    continue;
                }

                var productExists = await _productRepository.ExistsAsync(p => p.Id == productId);
                if (!productExists)
                {
                    rowErrors.Add($"Row {row.RowNumber()}: ProductId {productId} does not exist.");
                    continue;
                }

                var vendorExists = await _vendorRepository.ExistsAsync(v => v.URN_No == urn);
                if (!vendorExists)
                {
                    rowErrors.Add($"Row {row.RowNumber()}: Vendor with URN_No '{urn}' does not exist.");
                    continue;
                }

                var quantityOrdered = row.Cell(7).TryGetValue<decimal>(out var qo) ? qo : 0;
                var unitRate = row.Cell(8).TryGetValue<decimal>(out var ur) ? ur : 0;
                var quantitySupplied = row.Cell(9).TryGetValue<decimal>(out var qs) ? qs : 0;

                var supplyOrder = new SupplyOrder
                {
                    ProductId = productId,
                    PRO_No = row.Cell(2).GetString().Trim(),
                    PO_No = poNumber,
                    PO_Date = row.Cell(4).TryGetValue<DateTime>(out var poDate) ? poDate : null,
                    URN_No = urn,
                    Whether_MSME = row.Cell(6).GetString().Trim().Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                   row.Cell(6).GetString().Trim() == "1",
                    Qty_Ordered = quantityOrdered,
                    Unit_Rate = unitRate,
                    // Total_line_value is server-computed in DbContext.SaveChanges — not set here.
                    Qty_Supplied = quantitySupplied,
                    IsActive = true,
                    Manual_Bulk_upload = true
                };

                await _supplyOrderRepository.AddAsync(supplyOrder);
                successCount++;
            }
            catch (Exception ex)
            {
                rowErrors.Add($"Row {row.RowNumber()}: {ex.Message}");
            }
        }

        await _supplyOrderRepository.SaveChangesAsync();

        return await RecordUploadHistoryAsync(
            SyncEntityType.SupplyOrder, fileName, totalCount, successCount, rowErrors, uploadedByUserId);
    }

    public async Task<PagedResult<UploadHistoryResponseDto>> GetHistoryAsync(UploadHistoryFilterParams filter)
    {
        var (items, totalCount) = await _uploadHistoryRepository.GetPagedAsync(filter);

        return new PagedResult<UploadHistoryResponseDto>
        {
            Items = _mapper.Map<List<UploadHistoryResponseDto>>(items),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalCount
        };
    }

    private async Task<UploadResultDto> RecordUploadHistoryAsync(
        string entityType, string fileName, int totalCount, int successCount, List<string> rowErrors, int uploadedByUserId)
    {
        var failedCount = totalCount - successCount;
        var status = failedCount == 0
            ? UploadStatus.Completed
            : (successCount == 0 ? UploadStatus.Failed : UploadStatus.CompletedWithErrors);

        var history = new UploadHistory
        {
            UploadType = UploadType.Bulk,
            EntityType = entityType,
            FileName = fileName,
            TotalRecords = totalCount,
            SuccessRecords = successCount,
            FailedRecords = failedCount,
            UploadedBy = uploadedByUserId,
            UploadDate = DateTime.UtcNow,
            Status = status
        };

        await _uploadHistoryRepository.AddAsync(history);
        await _uploadHistoryRepository.SaveChangesAsync();

        return new UploadResultDto
        {
            UploadHistoryId = history.UploadHistoryId,
            FileName = fileName,
            EntityType = entityType,
            UploadType = UploadType.Bulk,
            TotalRecords = totalCount,
            SuccessRecords = successCount,
            FailedRecords = failedCount,
            Status = status,
            RowErrors = rowErrors
        };
    }
}