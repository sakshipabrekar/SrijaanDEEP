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

    /// <summary>
    /// GET api/vendors
    /// Paged list of vendors. Supports free-text SearchTerm and exact filters
    /// (URN_No, GST_No, PAN_No, MSME_No, IsActive).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<VendorResponseDto>>>> GetAll(
        [FromQuery] VendorFilterParams filter)
    {
        var result = await _vendorService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<VendorResponseDto>>
            .SuccessResponse(result, "Vendors fetched successfully."));
    }

    /// <summary>
    /// GET api/vendors/{urnNo}
    /// Gets a single vendor by URN_No (the primary key).
    /// </summary>
    [HttpGet("{urnNo}")]
    public async Task<ActionResult<ApiResponse<VendorResponseDto>>> GetByURNNo(string urnNo)
    {
        var vendor = await _vendorService.GetByURNNoAsync(urnNo);
        if (vendor is null)
            return NotFound(ApiResponse<VendorResponseDto>.FailureResponse("Vendor not found."));

        return Ok(ApiResponse<VendorResponseDto>.SuccessResponse(vendor, "Vendor fetched successfully."));
    }

    /// <summary>
    /// GET api/vendors/search?searchTerm=...
    /// Free-text search across Vendor_Org_Name, GST_No, PAN_No, URN_No, Nodal_Officer_Name.
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<VendorResponseDto>>>> Search(
        [FromQuery] string searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new VendorFilterParams
        {
            SearchTerm = searchTerm,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await _vendorService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<VendorResponseDto>>
            .SuccessResponse(result, "Vendors fetched successfully."));
    }

    /// <summary>
    /// GET api/vendors/filter
    /// Exact-match filter by GST_No / PAN_No / MSME_No / URN_No / IsActive.
    /// </summary>
    [HttpGet("filter")]
    public async Task<ActionResult<ApiResponse<PagedResult<VendorResponseDto>>>> Filter(
        [FromQuery] VendorFilterParams filter)
    {
        var result = await _vendorService.GetPagedAsync(filter);
        return Ok(ApiResponse<PagedResult<VendorResponseDto>>
            .SuccessResponse(result, "Vendors fetched successfully."));
    }

    /// <summary>
    /// POST api/vendors
    /// Creates a new vendor. URN_No is required in the body — it becomes the PK.
    /// Audit fields (Uploaded_by, Uploaded_DateTime, Upload_IP) are set automatically
    /// from the authenticated user and request context.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<VendorResponseDto>>> Create(
        [FromBody] CreateVendorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<VendorResponseDto>.FailureResponse("Validation failed."));

        var uploadedBy = User.Identity?.Name ?? "System";
        var uploadIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var (success, message, data) = await _vendorService.CreateAsync(dto, uploadedBy, uploadIp);
        if (!success)
            return BadRequest(ApiResponse<VendorResponseDto>.FailureResponse(message));

        return CreatedAtAction(
            nameof(GetByURNNo),
            new { urnNo = data!.URN_No },
            ApiResponse<VendorResponseDto>.SuccessResponse(data, message));
    }

    /// <summary>
    /// PUT api/vendors/{urnNo}
    /// Updates an existing vendor. Set IsActive=false to soft-deactivate.
    /// Audit fields (Last_Modified_by, Last_Modified_DateTime, Modify_IP) are set automatically.
    /// There is no DELETE endpoint — use IsActive=false instead.
    /// </summary>
    [HttpPut("{urnNo}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<VendorResponseDto>>> Update(
        string urnNo, [FromBody] UpdateVendorDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<VendorResponseDto>.FailureResponse("Validation failed."));

        var modifiedBy = User.Identity?.Name ?? "System";
        var modifyIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var (success, message, data) = await _vendorService.UpdateAsync(urnNo, dto, modifiedBy, modifyIp);
        if (!success)
        {
            return message == "Vendor not found."
                ? NotFound(ApiResponse<VendorResponseDto>.FailureResponse(message))
                : BadRequest(ApiResponse<VendorResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<VendorResponseDto>.SuccessResponse(data, message));
    }
}