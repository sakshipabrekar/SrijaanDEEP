using AutoMapper;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface ISupplyOrderService
{
    Task<PagedResult<SupplyOrderResponseDto>> GetPagedAsync(SupplyOrderFilterParams filter);
    Task<SupplyOrderResponseDto?> GetByIdAsync(int supplyOrderId);
    Task<(bool Success, string Message, SupplyOrderResponseDto? Data)> CreateAsync(CreateSupplyOrderDto dto);
    Task<(bool Success, string Message, SupplyOrderResponseDto? Data)> UpdateAsync(int supplyOrderId, UpdateSupplyOrderDto dto);
}

public class SupplyOrderService : ISupplyOrderService
{
    private readonly ISupplyOrderRepository _supplyOrderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public SupplyOrderService(
        ISupplyOrderRepository supplyOrderRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _supplyOrderRepository = supplyOrderRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

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

    public async Task<SupplyOrderResponseDto?> GetByIdAsync(int supplyOrderId)
    {
        var supplyOrder = await _supplyOrderRepository.GetByIdAsync(supplyOrderId);
        return supplyOrder is null ? null : _mapper.Map<SupplyOrderResponseDto>(supplyOrder);
    }

    public async Task<(bool Success, string Message, SupplyOrderResponseDto? Data)> CreateAsync(CreateSupplyOrderDto dto)
    {
        var productExists = await _productRepository.ExistsAsync(p => p.ProductId == dto.ProductId);
        if (!productExists)
        {
            return (false, $"Product with id {dto.ProductId} was not found.", null);
        }

        var supplyOrder = _mapper.Map<SupplyOrder>(dto);
        supplyOrder.TotalLineValue = dto.QuantityOrdered * dto.UnitRate;
        supplyOrder.IsActive = true;

        await _supplyOrderRepository.AddAsync(supplyOrder);
        await _supplyOrderRepository.SaveChangesAsync();

        var saved = await _supplyOrderRepository.GetByIdAsync(supplyOrder.SupplyOrderId);
        return (true, "Supply order created successfully.", _mapper.Map<SupplyOrderResponseDto>(saved));
    }

    public async Task<(bool Success, string Message, SupplyOrderResponseDto? Data)> UpdateAsync(int supplyOrderId, UpdateSupplyOrderDto dto)
    {
        var existing = await _supplyOrderRepository.GetByIdAsync(supplyOrderId);
        if (existing is null)
        {
            return (false, "Supply order not found.", null);
        }

        var productExists = await _productRepository.ExistsAsync(p => p.ProductId == dto.ProductId);
        if (!productExists)
        {
            return (false, $"Product with id {dto.ProductId} was not found.", null);
        }

        _mapper.Map(dto, existing);
        existing.TotalLineValue = dto.QuantityOrdered * dto.UnitRate;

        _supplyOrderRepository.Update(existing);
        await _supplyOrderRepository.SaveChangesAsync();

        var saved = await _supplyOrderRepository.GetByIdAsync(supplyOrderId);
        return (true, "Supply order updated successfully.", _mapper.Map<SupplyOrderResponseDto>(saved));
    }
}