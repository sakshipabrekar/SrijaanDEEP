using AutoMapper;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface ISupplyOrderService
{
    Task<PagedResult<SupplyOrderResponseDto>> GetPagedAsync(SupplyOrderFilterParams filter);
    Task<SupplyOrderResponseDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, SupplyOrderResponseDto? Data)> CreateAsync(CreateSupplyOrderDto dto, string uploadedBy, string uploadIp);
    Task<(bool Success, string Message, SupplyOrderResponseDto? Data)> UpdateAsync(int id, UpdateSupplyOrderDto dto, string modifiedBy, string modifyIp);
}

public class SupplyOrderService : ISupplyOrderService
{
    private readonly ISupplyOrderRepository _supplyOrderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;

    public SupplyOrderService(
        ISupplyOrderRepository supplyOrderRepository,
        IProductRepository productRepository,
        IVendorRepository vendorRepository,
        IMapper mapper)
    {
        _supplyOrderRepository = supplyOrderRepository;
        _productRepository = productRepository;
        _vendorRepository = vendorRepository;
        _mapper = mapper;
    }

    // ─── GET paged ────────────────────────────────────────────────────────────

    public async Task<PagedResult<SupplyOrderResponseDto>> GetPagedAsync(SupplyOrderFilterParams filter)
    {
        var (items, totalCount) = await _supplyOrderRepository.GetPagedAsync(filter);
        return new PagedResult<SupplyOrderResponseDto>
        {
            Items = _mapper.Map<List<SupplyOrderResponseDto>>(items),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalCount
        };
    }

    // ─── GET by id ────────────────────────────────────────────────────────────

    public async Task<SupplyOrderResponseDto?> GetByIdAsync(int id)
    {
        var so = await _supplyOrderRepository.GetByIdAsync(id);
        return so is null ? null : _mapper.Map<SupplyOrderResponseDto>(so);
    }

    // ─── CREATE ───────────────────────────────────────────────────────────────

    public async Task<(bool Success, string Message, SupplyOrderResponseDto? Data)> CreateAsync(
        CreateSupplyOrderDto dto, string uploadedBy, string uploadIp)
    {
        // Validate FK — Product must exist
        if (!await _productRepository.ExistsAsync(p => p.Id == dto.ProductId))
            return (false, $"Product with Id {dto.ProductId} was not found.", null);

        // Validate FK — Vendor (URN_No) must exist
        if (!await _vendorRepository.ExistsAsync(v => v.URN_No == dto.URN_No))
            return (false, $"Vendor with URN_No '{dto.URN_No}' was not found.", null);

        var supplyOrder = _mapper.Map<SupplyOrder>(dto);

        // Server-computed field
        supplyOrder.Total_line_value = dto.Qty_Ordered * dto.Unit_Rate;

        // Audit
        supplyOrder.IsActive = true;
        supplyOrder.Uploaded_by = uploadedBy;
        supplyOrder.Uploaded_DateTime = DateTime.UtcNow;
        supplyOrder.Upload_IP = uploadIp;

        await _supplyOrderRepository.AddAsync(supplyOrder);
        await _supplyOrderRepository.SaveChangesAsync();

        var saved = await _supplyOrderRepository.GetByIdAsync(supplyOrder.Id);
        return (true, "Supply order created successfully.", _mapper.Map<SupplyOrderResponseDto>(saved));
    }

    // ─── UPDATE ───────────────────────────────────────────────────────────────

    public async Task<(bool Success, string Message, SupplyOrderResponseDto? Data)> UpdateAsync(
        int id, UpdateSupplyOrderDto dto, string modifiedBy, string modifyIp)
    {
        var existing = await _supplyOrderRepository.GetByIdAsync(id);
        if (existing is null)
            return (false, "Supply order not found.", null);

        // Validate FK — Product must exist
        if (!await _productRepository.ExistsAsync(p => p.Id == dto.ProductId))
            return (false, $"Product with Id {dto.ProductId} was not found.", null);

        // Validate FK — Vendor (URN_No) must exist
        if (!await _vendorRepository.ExistsAsync(v => v.URN_No == dto.URN_No))
            return (false, $"Vendor with URN_No '{dto.URN_No}' was not found.", null);

        _mapper.Map(dto, existing);

        // Recompute server-side total
        existing.Total_line_value = dto.Qty_Ordered * dto.Unit_Rate;
        existing.Last_Modified_by = modifiedBy;
        existing.Last_Modified_DateTime = DateTime.UtcNow;
        existing.Modify_IP = modifyIp;

        _supplyOrderRepository.Update(existing);
        await _supplyOrderRepository.SaveChangesAsync();

        var saved = await _supplyOrderRepository.GetByIdAsync(id);
        return (true, "Supply order updated successfully.", _mapper.Map<SupplyOrderResponseDto>(saved));
    }
}