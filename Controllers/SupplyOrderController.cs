using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Services;

namespace SrijanDEEP.API.Controllers;

[ApiController]
[Route("api/supply-orders")]
[Produces("application/json")]
[Authorize]
public class SupplyOrderController : ControllerBase
{
    private readonly ISupplyOrderService _supplyOrderService;

    public SupplyOrderController(ISupplyOrderService supplyOrderService)
    {
        _supplyOrderService = supplyOrderService;
    }

    /// <summary>
    /// GET api/supply-orders
    /// Paged list. Supports free-text SearchTerm and exact filters
    /// (ProductId, URN_No, Whether_MSME, PO_DateFrom/To, IsActive).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplyOrderResponseDto>>>> GetAll(
        [FromQuery] SupplyOrderFilterParams filter)
    {
        var result = await _supplyOrderService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<SupplyOrderResponseDto>>
            .SuccessResponse(result, "Supply orders fetched successfully."));
    }

    /// <summary>
    /// GET api/supply-orders/{id}
    /// Gets a single supply order by its integer PK.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SupplyOrderResponseDto>>> GetById(int id)
    {
        var supplyOrder = await _supplyOrderService.GetByIdAsync(id);
        if (supplyOrder is null)
            return NotFound(ApiResponse<SupplyOrderResponseDto>.FailureResponse("Supply order not found."));

        return Ok(ApiResponse<SupplyOrderResponseDto>.SuccessResponse(supplyOrder, "Supply order fetched successfully."));
    }

    /// <summary>
    /// GET api/supply-orders/search?searchTerm=...
    /// Free-text search across PO_No, PRO_No, URN_No.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplyOrderResponseDto>>>> Search(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new SupplyOrderFilterParams
        {
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _supplyOrderService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<SupplyOrderResponseDto>>
            .SuccessResponse(result, "Supply orders fetched successfully."));
    }

    /// <summary>
    /// GET api/supply-orders/filter
    /// Exact-match filter by ProductId / URN_No / Whether_MSME / PO date range / IsActive.
    /// </summary>
    [HttpGet("filter")]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplyOrderResponseDto>>>> Filter(
        [FromQuery] SupplyOrderFilterParams filter)
    {
        var result = await _supplyOrderService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<SupplyOrderResponseDto>>
            .SuccessResponse(result, "Supply orders fetched successfully."));
    }

    /// <summary>
    /// POST api/supply-orders
    /// Creates a new supply order. Total_line_value = Qty_Ordered × Unit_Rate (server-computed).
    /// Both ProductId and URN_No must reference existing records.
    /// Audit fields set automatically from authenticated user and request context.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SupplyOrderResponseDto>>> Create(
        [FromBody] CreateSupplyOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<SupplyOrderResponseDto>.FailureResponse("Validation failed."));

        var uploadedBy = User.Identity?.Name ?? "System";
        var uploadIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var (success, message, data) = await _supplyOrderService.CreateAsync(dto, uploadedBy, uploadIp);
        if (!success)
            return BadRequest(ApiResponse<SupplyOrderResponseDto>.FailureResponse(message));

        return CreatedAtAction(
            nameof(GetById),
            new { id = data!.Id },
            ApiResponse<SupplyOrderResponseDto>.SuccessResponse(data, message));
    }

    /// <summary>
    /// PUT api/supply-orders/{id}
    /// Updates a supply order. Set IsActive=false to soft-deactivate — there is no DELETE endpoint.
    /// Total_line_value is recomputed server-side. Audit fields set automatically.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SupplyOrderResponseDto>>> Update(
        int id, [FromBody] UpdateSupplyOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<SupplyOrderResponseDto>.FailureResponse("Validation failed."));

        var modifiedBy = User.Identity?.Name ?? "System";
        var modifyIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var (success, message, data) = await _supplyOrderService.UpdateAsync(id, dto, modifiedBy, modifyIp);
        if (!success)
        {
            return message == "Supply order not found."
                ? NotFound(ApiResponse<SupplyOrderResponseDto>.FailureResponse(message))
                : BadRequest(ApiResponse<SupplyOrderResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<SupplyOrderResponseDto>.SuccessResponse(data, message));
    }
}