using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

// ─── Request ─────────────────────────────────────────────────────────────────

public class TriggerSyncRequestDto
{
    /// <summary>Vendor / Product / SupplyOrder — see SyncEntityType.</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Date sent to the DPSU portal as ?last_updated_date=dd-MM-yyyy.
    /// Records whose Last_Modified_Date is after this date are candidates for sync.
    /// </summary>
    public DateTime LastUpdatedDate { get; set; }
}

// ─── List / paged response ────────────────────────────────────────────────────

public class SyncHistoryResponseDto
{
    public int SyncHistoryId { get; set; }
    public DateTime SyncDate { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RecordsProcessed { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }

    // Human-readable outcome for each sync run
    public string SyncMessage { get; set; } = string.Empty;
}

/// <summary>Full detail view including error/response payload.</summary>
public class SyncHistoryDetailDto : SyncHistoryResponseDto
{
    public string? ErrorDescription { get; set; }
    public string? ResponseData { get; set; }
}

// ─── Per-record classification returned inside the trigger response ───────────

public class SyncRecordResultDto
{
    public string RecordId { get; set; } = string.Empty;
    public string SyncCategory { get; set; } = string.Empty;  // enum name
    public string SyncMessage { get; set; } = string.Empty;   // human-readable label
    public bool IsSynced { get; set; }
    public DateTime LastModifiedDate { get; set; }
}

public class TriggerSyncResponseDto
{
    public int SyncHistoryId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public DateTime LastUpdatedDate { get; set; }
    public string OverallStatus { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int RecordsProcessed { get; set; }
    public List<SyncRecordResultDto> RecordResults { get; set; } = new();
}

// ─── Filter params ────────────────────────────────────────────────────────────

public class SyncHistoryFilterParams : PaginationParams
{
    public string? EntityType { get; set; }
    public string? Status { get; set; }
    public DateTime? SyncDateFrom { get; set; }
    public DateTime? SyncDateTo { get; set; }
}