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

    /// <summary>
    /// GET api/sync/status
    /// Paged list of sync runs, optionally filtered by EntityType / Status / date range.
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse<PagedResult<SyncHistoryResponseDto>>>> GetSyncStatusList(
        [FromQuery] SyncHistoryFilterParams filter)
    {
        var result = await _syncService.GetSyncStatusListAsync(filter);
        return Ok(ApiResponse<PagedResult<SyncHistoryResponseDto>>
            .SuccessResponse(result, "Sync status list fetched successfully."));
    }

    /// <summary>
    /// GET api/sync/status/{id}
    /// Full detail for a single sync run (includes ResponseData).
    /// </summary>
    [HttpGet("status/{id:int}")]
    public async Task<ActionResult<ApiResponse<SyncHistoryDetailDto>>> GetSyncStatusDetail(int id)
    {
        var detail = await _syncService.GetSyncStatusDetailAsync(id);
        if (detail is null)
            return NotFound(ApiResponse<SyncHistoryDetailDto>
                .FailureResponse("Sync history record not found."));

        return Ok(ApiResponse<SyncHistoryDetailDto>
            .SuccessResponse(detail, "Sync status detail fetched successfully."));
    }

    /// <summary>
    /// GET api/sync/status/{id}/error
    /// Returns only the ErrorDescription / ResponseData for a failed sync run.
    /// </summary>
    [HttpGet("status/{id:int}/error")]
    public async Task<ActionResult<ApiResponse<SyncHistoryDetailDto>>> GetErrorDescription(int id)
    {
        var detail = await _syncService.GetErrorDescriptionAsync(id);
        if (detail is null)
            return NotFound(ApiResponse<SyncHistoryDetailDto>
                .FailureResponse("Sync history record not found."));

        return Ok(ApiResponse<SyncHistoryDetailDto>
            .SuccessResponse(detail, "Error description fetched successfully."));
    }

    /// <summary>
    /// POST api/sync/trigger
    /// Triggers a manual sync for the selected entity type and cut-off date.
    ///
    /// Body:
    /// {
    ///   "entityType": "Product",          // Vendor | Product | SupplyOrder
    ///   "lastUpdatedDate": "2026-06-21"   // ISO date; sent to DPSU as dd-MM-yyyy
    /// }
    ///
    /// Each record returned by the DPSU portal is classified as:
    ///   • NotRequired           — already synced, not modified after cut-off
    ///   • ModificationToSync    — synced before, modified after cut-off → re-sync needed
    ///   • FailedDuringLastSync  — not synced, not modified after cut-off → previous failure
    ///   • FirstTimeSync         — not synced, modified after cut-off → new record
    /// </summary>
    [HttpPost("trigger")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<TriggerSyncResponseDto>>> TriggerManualSync(
        [FromBody] TriggerSyncRequestDto request)
    {
        var (success, message, data) = await _syncService.TriggerManualSyncAsync(request);

        if (!success)
            return BadRequest(ApiResponse<TriggerSyncResponseDto>
                .FailureResponse(message));

        return Ok(ApiResponse<TriggerSyncResponseDto>
            .SuccessResponse(data, message));
    }
}