using AutoMapper;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface IProductService
{
    Task<PagedResult<ProductResponseDto>> GetPagedAsync(ProductFilterParams filter);
    Task<ProductResponseDto?> GetByIdAsync(int id);
    Task<(bool Success, string Message, ProductResponseDto? Data)> CreateAsync(CreateProductDto dto, string uploadedBy, string uploadIp);
    Task<(bool Success, string Message, ProductResponseDto? Data)> UpdateAsync(int id, UpdateProductDto dto, string modifiedBy, string modifyIp);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IVendorRepository vendorRepository,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _vendorRepository = vendorRepository;
        _mapper = mapper;
    }

    // ─── GET paged ────────────────────────────────────────────────────────────

    public async Task<PagedResult<ProductResponseDto>> GetPagedAsync(ProductFilterParams filter)
    {
        var (items, totalCount) = await _productRepository.GetPagedAsync(filter);
        return new PagedResult<ProductResponseDto>
        {
            Items = _mapper.Map<List<ProductResponseDto>>(items),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalCount
        };
    }

    // ─── GET by id ────────────────────────────────────────────────────────────

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product is null ? null : _mapper.Map<ProductResponseDto>(product);
    }

    // ─── CREATE ───────────────────────────────────────────────────────────────

    public async Task<(bool Success, string Message, ProductResponseDto? Data)> CreateAsync(
        CreateProductDto dto, string uploadedBy, string uploadIp)
    {
        // Validate vendor FK (URN_No)
        if (!await _vendorRepository.ExistsAsync(v => v.URN_No == dto.URN_No))
            return (false, $"Vendor with URN_No '{dto.URN_No}' was not found.", null);

        var product = _mapper.Map<Product>(dto);
        product.IsActive = true;
        product.Uploaded_by = uploadedBy;
        product.Uploaded_DateTime = DateTime.UtcNow;
        product.Upload_IP = uploadIp;

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        var saved = await _productRepository.GetByIdAsync(product.Id);
        return (true, "Product created successfully.", _mapper.Map<ProductResponseDto>(saved));
    }

    // ─── UPDATE ───────────────────────────────────────────────────────────────

    public async Task<(bool Success, string Message, ProductResponseDto? Data)> UpdateAsync(
        int id, UpdateProductDto dto, string modifiedBy, string modifyIp)
    {
        var existing = await _productRepository.GetByIdAsync(id);
        if (existing is null)
            return (false, "Product not found.", null);

        // Validate vendor FK if changed
        if (dto.URN_No != existing.URN_No &&
            !await _vendorRepository.ExistsAsync(v => v.URN_No == dto.URN_No))
        {
            return (false, $"Vendor with URN_No '{dto.URN_No}' was not found.", null);
        }

        _mapper.Map(dto, existing);
        existing.Last_Modified_by = modifiedBy;
        existing.Last_Modified_DateTime = DateTime.UtcNow;
        existing.Modify_IP = modifyIp;

        _productRepository.Update(existing);
        await _productRepository.SaveChangesAsync();

        var saved = await _productRepository.GetByIdAsync(id);
        return (true, "Product updated successfully.", _mapper.Map<ProductResponseDto>(saved));
    }
}