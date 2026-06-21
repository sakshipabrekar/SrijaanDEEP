using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Services;

namespace SrijanDEEP.API.Controllers;

[ApiController]
[Route("api/vendors")]
[Produces("application/json")]
[Authorize]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    /// <summary>Gets all vendors. Supports paging via PageNumber/PageSize and free-text search via SearchTerm.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<VendorResponseDto>>>> GetAll([FromQuery] VendorFilterParams filter)
    {
        var result = await _vendorService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<VendorResponseDto>>.SuccessResponse(result, "Vendors fetched successfully."));
    }

    /// <summary>Gets a single vendor by id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<VendorResponseDto>>> GetById(int id)
    {
        var vendor = await _vendorService.GetByIdAsync(id);
        if (vendor is null)
        {
            return NotFound(ApiResponse<VendorResponseDto>.FailureResponse("Vendor not found."));
        }

        return Ok(ApiResponse<VendorResponseDto>.SuccessResponse(vendor, "Vendor fetched successfully."));
    }

    /// <summary>
    /// Searches vendors by a free-text term across organisation name, GST, PAN, URN and nodal officer name.
    /// This is the same engine as GetAll's SearchTerm - exposed as a distinct, discoverable route.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<VendorResponseDto>>>> Search(
        [FromQuery] string searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var filter = new VendorFilterParams { SearchTerm = searchTerm, PageNumber = pageNumber, PageSize = pageSize };
        var result = await _vendorService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<VendorResponseDto>>.SuccessResponse(result, "Vendors fetched successfully."));
    }

    /// <summary>Filters vendors by GST number, PAN number, MSME number and/or active status.</summary>
    [HttpGet("filter")]
    public async Task<ActionResult<ApiResponse<PagedResult<VendorResponseDto>>>> Filter([FromQuery] VendorFilterParams filter)
    {
        var result = await _vendorService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<VendorResponseDto>>.SuccessResponse(result, "Vendors fetched successfully."));
    }

    /// <summary>Creates a new vendor.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<VendorResponseDto>>> Create([FromBody] CreateVendorDto dto)
    {
        var (success, message, data) = await _vendorService.CreateAsync(dto);
        if (!success)
        {
            return BadRequest(ApiResponse<VendorResponseDto>.FailureResponse(message));
        }

        return CreatedAtAction(nameof(GetById), new { id = data!.VendorId }, ApiResponse<VendorResponseDto>.SuccessResponse(data, message));
    }

    /// <summary>
    /// Updates a vendor. There is no delete endpoint - set IsActive=false here to deactivate
    /// a vendor without ever removing the record.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<VendorResponseDto>>> Update(int id, [FromBody] UpdateVendorDto dto)
    {
        var (success, message, data) = await _vendorService.UpdateAsync(id, dto);
        if (!success)
        {
            return data is null && message == "Vendor not found."
                ? NotFound(ApiResponse<VendorResponseDto>.FailureResponse(message))
                : BadRequest(ApiResponse<VendorResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<VendorResponseDto>.SuccessResponse(data, message));
    }
}