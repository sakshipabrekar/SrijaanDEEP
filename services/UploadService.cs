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
    // URNNumber | CINNumber | MSMENumber | GSTNumber | PANNumber | VendorOrganisationName |
    // VendorOrganisationEmail | NodalOfficerName | NodalOfficerEmail | NodalOfficerMobile |
    // NodalOfficerDesignation | VendorCodeAssignedByDPSU
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

                if (string.IsNullOrWhiteSpace(orgName))
                {
                    rowErrors.Add($"Row {row.RowNumber()}: VendorOrganisationName is required.");
                    continue;
                }

                var existing = !string.IsNullOrWhiteSpace(urn)
                    ? await _vendorRepository.GetByURNNumberAsync(urn)
                    : null;

                var vendor = existing ?? new Vendor();
                vendor.URNNumber = string.IsNullOrWhiteSpace(urn) ? vendor.URNNumber : urn;
                vendor.CINNumber = row.Cell(2).GetString().Trim();
                vendor.MSMENumber = row.Cell(3).GetString().Trim();
                vendor.GSTNumber = row.Cell(4).GetString().Trim();
                vendor.PANNumber = row.Cell(5).GetString().Trim();
                vendor.VendorOrganisationName = orgName;
                vendor.VendorOrganisationEmail = row.Cell(7).GetString().Trim();
                vendor.NodalOfficerName = row.Cell(8).GetString().Trim();
                vendor.NodalOfficerEmail = row.Cell(9).GetString().Trim();
                vendor.NodalOfficerMobile = row.Cell(10).GetString().Trim();
                vendor.NodalOfficerDesignation = row.Cell(11).GetString().Trim();
                vendor.VendorCodeAssignedByDPSU = row.Cell(12).GetString().Trim();
                vendor.IsActive = true;

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

    // Expected columns: UniqueReferenceNumber | CompanyName | ProductName | DefencePlatform |
    // ProductType | PartNumber | IsItemSupplied | SupplyOrderNumber | SupplyOrderDate | VendorId
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
                var productName = row.Cell(3).GetString().Trim();
                var vendorId = row.Cell(10).GetValue<int>();

                if (string.IsNullOrWhiteSpace(urn) || string.IsNullOrWhiteSpace(productName))
                {
                    rowErrors.Add($"Row {row.RowNumber()}: UniqueReferenceNumber and ProductName are required.");
                    continue;
                }

                var vendorExists = await _vendorRepository.ExistsAsync(v => v.VendorId == vendorId);
                if (!vendorExists)
                {
                    rowErrors.Add($"Row {row.RowNumber()}: VendorId {vendorId} does not exist.");
                    continue;
                }

                var existing = await _productRepository.GetByUniqueReferenceNumberAsync(urn);
                var product = existing ?? new Product();
                product.UniqueReferenceNumber = urn;
                product.CompanyName = row.Cell(2).GetString().Trim();
                product.ProductName = productName;
                product.DefencePlatform = row.Cell(4).GetString().Trim();
                product.ProductType = row.Cell(5).GetString().Trim();
                product.PartNumber = row.Cell(6).GetString().Trim();
                product.IsItemSupplied = row.Cell(7).GetString().Trim().Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                          row.Cell(7).GetString().Trim() == "1";
                product.SupplyOrderNumber = row.Cell(8).GetString().Trim();
                product.SupplyOrderDate = row.Cell(9).TryGetValue<DateTime>(out var soDate) ? soDate : null;
                product.VendorId = vendorId;
                product.IsActive = true;

                if (existing is null)
                {
                    await _productRepository.AddAsync(product);
                }
                else
                {
                    _productRepository.Update(product);
                }

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

    // Expected columns: ProductId | PurchaseOrderNumber | PurchaseOrderDate | VendorName |
    // URNNumber | IsVendorMSME | QuantityOrdered | UnitRate | QuantitySupplied
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
                var poNumber = row.Cell(2).GetString().Trim();

                if (productId <= 0 || string.IsNullOrWhiteSpace(poNumber))
                {
                    rowErrors.Add($"Row {row.RowNumber()}: ProductId and PurchaseOrderNumber are required.");
                    continue;
                }

                var productExists = await _productRepository.ExistsAsync(p => p.ProductId == productId);
                if (!productExists)
                {
                    rowErrors.Add($"Row {row.RowNumber()}: ProductId {productId} does not exist.");
                    continue;
                }

                var quantityOrdered = row.Cell(7).TryGetValue<decimal>(out var qo) ? qo : 0;
                var unitRate = row.Cell(8).TryGetValue<decimal>(out var ur) ? ur : 0;
                var quantitySupplied = row.Cell(9).TryGetValue<decimal>(out var qs) ? qs : 0;

                var supplyOrder = new SupplyOrder
                {
                    ProductId = productId,
                    PurchaseOrderNumber = poNumber,
                    PurchaseOrderDate = row.Cell(3).TryGetValue<DateTime>(out var poDate) ? poDate : null,
                    VendorName = row.Cell(4).GetString().Trim(),
                    URNNumber = row.Cell(5).GetString().Trim(),
                    IsVendorMSME = row.Cell(6).GetString().Trim().Equals("true", StringComparison.OrdinalIgnoreCase) ||
                                   row.Cell(6).GetString().Trim() == "1",
                    QuantityOrdered = quantityOrdered,
                    UnitRate = unitRate,
                    TotalLineValue = quantityOrdered * unitRate,
                    QuantitySupplied = quantitySupplied,
                    IsActive = true
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