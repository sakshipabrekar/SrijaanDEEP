using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

public class SyncHistoryResponseDto
{
    public int SyncHistoryId { get; set; }
    public DateTime SyncDate { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RecordsProcessed { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>Full detail view, including the error/response payload omitted from the list view.</summary>
public class SyncHistoryDetailDto : SyncHistoryResponseDto
{
    public string? ErrorDescription { get; set; }
    public string? ResponseData { get; set; }
}

public class SyncHistoryFilterParams : PaginationParams
{
    public string? EntityType { get; set; }
    public string? Status { get; set; }
    public DateTime? SyncDateFrom { get; set; }
    public DateTime? SyncDateTo { get; set; }
}

public class TriggerSyncRequestDto
{
    /// <summary>Vendor / Product / SupplyOrder - see <see cref="Entities.SyncEntityType"/>.</summary>
    public string EntityType { get; set; } = string.Empty;
}