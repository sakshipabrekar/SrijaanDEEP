using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Services;

namespace SrijanDEEP.API.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>Gets all products. Supports paging via PageNumber/PageSize and free-text search via SearchTerm.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDto>>>> GetAll([FromQuery] ProductFilterParams filter)
    {
        var result = await _productService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<ProductResponseDto>>.SuccessResponse(result, "Products fetched successfully."));
    }

    /// <summary>Gets a single product by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound(ApiResponse<ProductResponseDto>.FailureResponse("Product not found."));
        }

        return Ok(ApiResponse<ProductResponseDto>.SuccessResponse(product, "Product fetched successfully."));
    }

    /// <summary>Searches products by a free-text term across product name, URN, company name, part number, platform.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDto>>>> Search(
        [FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var filter = new ProductFilterParams { SearchTerm = searchTerm, PageNumber = pageNumber, PageSize = pageSize };
        var result = await _productService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<ProductResponseDto>>.SuccessResponse(result, "Products fetched successfully."));
    }

    /// <summary>Filters products by VendorId, DefencePlatform, ProductType, IsItemSupplied and/or active status.</summary>
    [HttpGet("filter")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDto>>>> Filter([FromQuery] ProductFilterParams filter)
    {
        var result = await _productService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<ProductResponseDto>>.SuccessResponse(result, "Products fetched successfully."));
    }

    /// <summary>Creates a new product, linked to an existing vendor.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create([FromBody] CreateProductDto dto)
    {
        var (success, message, data) = await _productService.CreateAsync(dto);
        if (!success)
        {
            return BadRequest(ApiResponse<ProductResponseDto>.FailureResponse(message));
        }

        return CreatedAtAction(nameof(GetById), new { id = data!.ProductId }, ApiResponse<ProductResponseDto>.SuccessResponse(data, message));
    }

    /// <summary>
    /// Updates a product. There is no delete endpoint - set IsActive=false here to deactivate
    /// a product without ever removing the record.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        var (success, message, data) = await _productService.UpdateAsync(id, dto);
        if (!success)
        {
            return data is null && message == "Product not found."
                ? NotFound(ApiResponse<ProductResponseDto>.FailureResponse(message))
                : BadRequest(ApiResponse<ProductResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<ProductResponseDto>.SuccessResponse(data, message));
    }
}