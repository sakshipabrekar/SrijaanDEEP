using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Services;

namespace SrijanDEEP.API.Controllers;

[ApiController]
[Route("api/sync")]
[Produces("application/json")]
[Authorize]
public class SyncController : ControllerBase
{
    private readonly ISyncService _syncService;

    public SyncController(ISyncService syncService)
    {
        _syncService = syncService;
    }

    /// <summary>Gets a paged list of sync runs, optionally filtered by EntityType/Status/date range.</summary>
    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse<PagedResult<SyncHistoryResponseDto>>>> GetSyncStatusList([FromQuery] SyncHistoryFilterParams filter)
    {
        var result = await _syncService.GetSyncStatusListAsync(filter);
        return Ok(ApiResponse<PagedResult<SyncHistoryResponseDto>>.SuccessResponse(result, "Sync status list fetched successfully."));
    }

    /// <summary>Gets full detail (including ResponseData/ErrorDescription) for a single sync run.</summary>
    [HttpGet("status/{id:int}")]
    public async Task<ActionResult<ApiResponse<SyncHistoryDetailDto>>> GetSyncStatusDetail(int id)
    {
        var detail = await _syncService.GetSyncStatusDetailAsync(id);
        if (detail is null)
        {
            return NotFound(ApiResponse<SyncHistoryDetailDto>.FailureResponse("Sync history record not found."));
        }

        return Ok(ApiResponse<SyncHistoryDetailDto>.SuccessResponse(detail, "Sync status detail fetched successfully."));
    }

    /// <summary>Gets just the error description/response payload for a given sync run (useful for support/debugging UIs).</summary>
    [HttpGet("status/{id:int}/error")]
    public async Task<ActionResult<ApiResponse<SyncHistoryDetailDto>>> GetErrorDescription(int id)
    {
        var detail = await _syncService.GetErrorDescriptionAsync(id);
        if (detail is null)
        {
            return NotFound(ApiResponse<SyncHistoryDetailDto>.FailureResponse("Sync history record not found."));
        }

        return Ok(ApiResponse<SyncHistoryDetailDto>.SuccessResponse(detail, "Error description fetched successfully."));
    }

    /// <summary>Triggers a manual sync run for the given entity type (Vendor / Product / SupplyOrder).</summary>
    [HttpPost("trigger")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SyncHistoryResponseDto>>> TriggerManualSync([FromBody] TriggerSyncRequestDto request)
    {
        var (success, message, data) = await _syncService.TriggerManualSyncAsync(request);
        if (!success)
        {
            return BadRequest(ApiResponse<SyncHistoryResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<SyncHistoryResponseDto>.SuccessResponse(data, message));
    }
}