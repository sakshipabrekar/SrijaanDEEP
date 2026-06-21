using AutoMapper;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface IProductService
{
    Task<PagedResult<ProductResponseDto>> GetPagedAsync(ProductFilterParams filter);
    Task<ProductResponseDto?> GetByIdAsync(int productId);
    Task<(bool Success, string Message, ProductResponseDto? Data)> CreateAsync(CreateProductDto dto);
    Task<(bool Success, string Message, ProductResponseDto? Data)> UpdateAsync(int productId, UpdateProductDto dto);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IVendorRepository vendorRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _vendorRepository = vendorRepository;
        _mapper = mapper;
    }

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

    public async Task<ProductResponseDto?> GetByIdAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        return product is null ? null : _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<(bool Success, string Message, ProductResponseDto? Data)> CreateAsync(CreateProductDto dto)
    {
        var vendorExists = await _vendorRepository.ExistsAsync(v => v.VendorId == dto.VendorId);
        if (!vendorExists)
        {
            return (false, $"Vendor with id {dto.VendorId} was not found.", null);
        }

        if (await _productRepository.ExistsAsync(p => p.UniqueReferenceNumber == dto.UniqueReferenceNumber))
        {
            return (false, $"A product with unique reference number '{dto.UniqueReferenceNumber}' already exists.", null);
        }

        var product = _mapper.Map<Product>(dto);
        product.IsActive = true;

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        var saved = await _productRepository.GetByIdAsync(product.ProductId);
        return (true, "Product created successfully.", _mapper.Map<ProductResponseDto>(saved));
    }

    public async Task<(bool Success, string Message, ProductResponseDto? Data)> UpdateAsync(int productId, UpdateProductDto dto)
    {
        var existing = await _productRepository.GetByIdAsync(productId);
        if (existing is null)
        {
            return (false, "Product not found.", null);
        }

        var vendorExists = await _vendorRepository.ExistsAsync(v => v.VendorId == dto.VendorId);
        if (!vendorExists)
        {
            return (false, $"Vendor with id {dto.VendorId} was not found.", null);
        }

        if (dto.UniqueReferenceNumber != existing.UniqueReferenceNumber &&
            await _productRepository.ExistsAsync(p => p.UniqueReferenceNumber == dto.UniqueReferenceNumber && p.ProductId != productId))
        {
            return (false, $"A product with unique reference number '{dto.UniqueReferenceNumber}' already exists.", null);
        }

        _mapper.Map(dto, existing);
        _productRepository.Update(existing);
        await _productRepository.SaveChangesAsync();

        var saved = await _productRepository.GetByIdAsync(productId);
        return (true, "Product updated successfully.", _mapper.Map<ProductResponseDto>(saved));
    }
}