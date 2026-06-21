using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Services;

namespace SrijanDEEP.API.Controllers;

[ApiController]
[Route("api/upload")]
[Produces("application/json")]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;
    private readonly long _maxFileSizeBytes;
    private readonly string[] _allowedExtensions;

    public UploadController(IUploadService uploadService, IConfiguration configuration)
    {
        _uploadService = uploadService;
        var maxMb = int.TryParse(configuration["FileUploadSettings:MaxFileSizeMb"], out var mb) ? mb : 10;
        _maxFileSizeBytes = maxMb * 1024L * 1024L;
        _allowedExtensions = configuration.GetSection("FileUploadSettings:AllowedExtensions").Get<string[]>()
                              ?? new[] { ".xlsx", ".xls" };
    }

    /// <summary>Uploads an Excel file containing Vendor records (create or update by URNNumber).</summary>
    [HttpPost("vendors")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UploadResultDto>>> UploadVendorExcel(IFormFile file)
    {
        var validation = ValidateFile(file);
        if (validation is not null) return validation;

        var userId = GetUserId();
        using var stream = file.OpenReadStream();
        var result = await _uploadService.UploadVendorExcelAsync(stream, file.FileName, userId);

        return Ok(ApiResponse<UploadResultDto>.SuccessResponse(result, "Vendor file processed."));
    }

    /// <summary>Uploads an Excel file containing Product records (create or update by UniqueReferenceNumber).</summary>
    [HttpPost("products")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UploadResultDto>>> UploadProductExcel(IFormFile file)
    {
        var validation = ValidateFile(file);
        if (validation is not null) return validation;

        var userId = GetUserId();
        using var stream = file.OpenReadStream();
        var result = await _uploadService.UploadProductExcelAsync(stream, file.FileName, userId);

        return Ok(ApiResponse<UploadResultDto>.SuccessResponse(result, "Product file processed."));
    }

    /// <summary>Uploads an Excel file containing Supply Order records (each row creates a new supply order).</summary>
    [HttpPost("supply-orders")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UploadResultDto>>> UploadSupplyOrderExcel(IFormFile file)
    {
        var validation = ValidateFile(file);
        if (validation is not null) return validation;

        var userId = GetUserId();
        using var stream = file.OpenReadStream();
        var result = await _uploadService.UploadSupplyOrderExcelAsync(stream, file.FileName, userId);

        return Ok(ApiResponse<UploadResultDto>.SuccessResponse(result, "Supply order file processed."));
    }

    /// <summary>Gets paged upload history, optionally filtered by EntityType/Status/date range.</summary>
    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<PagedResult<UploadHistoryResponseDto>>>> GetHistory([FromQuery] UploadHistoryFilterParams filter)
    {
        var result = await _uploadService.GetHistoryAsync(filter);
        return Ok(ApiResponse<PagedResult<UploadHistoryResponseDto>>.SuccessResponse(result, "Upload history fetched successfully."));
    }

    private ActionResult? ValidateFile(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse.Fail("A file is required."));
        }

        if (file.Length > _maxFileSizeBytes)
        {
            return BadRequest(ApiResponse.Fail($"File exceeds the maximum allowed size of {_maxFileSizeBytes / (1024 * 1024)} MB."));
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            return BadRequest(ApiResponse.Fail($"Unsupported file type. Allowed types: {string.Join(", ", _allowedExtensions)}."));
        }

        return null;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}