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

    /// <summary>Gets all supply orders. Supports paging via PageNumber/PageSize and free-text search via SearchTerm.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplyOrderResponseDto>>>> GetAll([FromQuery] SupplyOrderFilterParams filter)
    {
        var result = await _supplyOrderService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<SupplyOrderResponseDto>>.SuccessResponse(result, "Supply orders fetched successfully."));
    }

    /// <summary>Gets a single supply order by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<SupplyOrderResponseDto>>> GetById(int id)
    {
        var supplyOrder = await _supplyOrderService.GetByIdAsync(id);
        if (supplyOrder is null)
        {
            return NotFound(ApiResponse<SupplyOrderResponseDto>.FailureResponse("Supply order not found."));
        }

        return Ok(ApiResponse<SupplyOrderResponseDto>.SuccessResponse(supplyOrder, "Supply order fetched successfully."));
    }

    /// <summary>Searches supply orders by a free-text term across purchase order number, vendor name and URN.</summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplyOrderResponseDto>>>> Search(
        [FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var filter = new SupplyOrderFilterParams { SearchTerm = searchTerm, PageNumber = pageNumber, PageSize = pageSize };
        var result = await _supplyOrderService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<SupplyOrderResponseDto>>.SuccessResponse(result, "Supply orders fetched successfully."));
    }

    /// <summary>Filters supply orders by ProductId, IsVendorMSME, purchase order date range and/or active status.</summary>
    [HttpGet("filter")]
    public async Task<ActionResult<ApiResponse<PagedResult<SupplyOrderResponseDto>>>> Filter([FromQuery] SupplyOrderFilterParams filter)
    {
        var result = await _supplyOrderService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<SupplyOrderResponseDto>>.SuccessResponse(result, "Supply orders fetched successfully."));
    }

    /// <summary>Creates a new supply order, linked to an existing product. TotalLineValue is computed server-side.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SupplyOrderResponseDto>>> Create([FromBody] CreateSupplyOrderDto dto)
    {
        var (success, message, data) = await _supplyOrderService.CreateAsync(dto);
        if (!success)
        {
            return BadRequest(ApiResponse<SupplyOrderResponseDto>.FailureResponse(message));
        }

        return CreatedAtAction(nameof(GetById), new { id = data!.SupplyOrderId }, ApiResponse<SupplyOrderResponseDto>.SuccessResponse(data, message));
    }

    /// <summary>
    /// Updates a supply order. There is no delete endpoint - set IsActive=false here to
    /// deactivate a supply order without ever removing the record.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SupplyOrderResponseDto>>> Update(int id, [FromBody] UpdateSupplyOrderDto dto)
    {
        var (success, message, data) = await _supplyOrderService.UpdateAsync(id, dto);
        if (!success)
        {
            return data is null && message == "Supply order not found."
                ? NotFound(ApiResponse<SupplyOrderResponseDto>.FailureResponse(message))
                : BadRequest(ApiResponse<SupplyOrderResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<SupplyOrderResponseDto>.SuccessResponse(data, message));
    }
}