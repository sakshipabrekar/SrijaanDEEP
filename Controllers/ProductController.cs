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

    /// <summary>
    /// GET api/products
    /// Paged list. Supports free-text SearchTerm and exact filters
    /// (URN_No, Defence_Platform, Product_Type, Is_Item_Supplied, IsActive).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDto>>>> GetAll(
        [FromQuery] ProductFilterParams filter)
    {
        var result = await _productService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<ProductResponseDto>>
            .SuccessResponse(result, "Products fetched successfully."));
    }

    /// <summary>
    /// GET api/products/{id}
    /// Gets a single product by its integer PK.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product is null)
            return NotFound(ApiResponse<ProductResponseDto>.FailureResponse("Product not found."));

        return Ok(ApiResponse<ProductResponseDto>.SuccessResponse(product, "Product fetched successfully."));
    }

    /// <summary>
    /// GET api/products/search?searchTerm=...
    /// Free-text search across Product_Name, Part_No, Defence_Platform, URN_No.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDto>>>> Search(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new ProductFilterParams
        {
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _productService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<ProductResponseDto>>
            .SuccessResponse(result, "Products fetched successfully."));
    }

    /// <summary>
    /// GET api/products/filter
    /// Exact-match filter by URN_No / Defence_Platform / Product_Type / Is_Item_Supplied / IsActive.
    /// </summary>
    [HttpGet("filter")]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductResponseDto>>>> Filter(
        [FromQuery] ProductFilterParams filter)
    {
        var result = await _productService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<ProductResponseDto>>
            .SuccessResponse(result, "Products fetched successfully."));
    }

    /// <summary>
    /// POST api/products
    /// Creates a new product linked to an existing vendor via URN_No.
    /// Audit fields (Uploaded_by, Uploaded_DateTime, Upload_IP) are set automatically.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Create(
        [FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ProductResponseDto>.FailureResponse("Validation failed."));

        var uploadedBy = User.Identity?.Name ?? "System";
        var uploadIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var (success, message, data) = await _productService.CreateAsync(dto, uploadedBy, uploadIp);
        if (!success)
            return BadRequest(ApiResponse<ProductResponseDto>.FailureResponse(message));

        return CreatedAtAction(
            nameof(GetById),
            new { id = data!.Id },
            ApiResponse<ProductResponseDto>.SuccessResponse(data, message));
    }

    /// <summary>
    /// PUT api/products/{id}
    /// Updates a product. Set IsActive=false to soft-deactivate — there is no DELETE endpoint.
    /// Audit fields (Last_Modified_by, Last_Modified_DateTime, Modify_IP) are set automatically.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ProductResponseDto>>> Update(
        int id, [FromBody] UpdateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<ProductResponseDto>.FailureResponse("Validation failed."));

        var modifiedBy = User.Identity?.Name ?? "System";
        var modifyIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var (success, message, data) = await _productService.UpdateAsync(id, dto, modifiedBy, modifyIp);
        if (!success)
        {
            return message == "Product not found."
                ? NotFound(ApiResponse<ProductResponseDto>.FailureResponse(message))
                : BadRequest(ApiResponse<ProductResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<ProductResponseDto>.SuccessResponse(data, message));
    }
}