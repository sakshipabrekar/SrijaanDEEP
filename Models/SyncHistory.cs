namespace SrijanDEEP.API.Entities;

// ─── Allowed entity types ─────────────────────────────────────────────────────

public static class SyncEntityType
{
    public const string Vendor = "Vendor";
    public const string Product = "Product";
    public const string SupplyOrder = "SupplyOrder";
}

// ─── Sync run status ──────────────────────────────────────────────────────────

public static class SyncStatus
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Success = "Success";
    public const string Failed = "Failed";
    public const string PartialSuccess = "PartialSuccess";
}

// ─── Per-record classification ────────────────────────────────────────────────

/// <summary>
/// Four-quadrant classification of each record returned by the DPSU portal.
///
///  IsSynced | LastModifiedDate vs LastUpdatedDate | Category
///  ---------|--------------------------------------|----------------------------
///  true     | <=                                  | NotRequired
///  true     | >                                   | ModificationToSync
///  false    | <=                                  | FailedDuringLastSync
///  false    | >                                   | FirstTimeSync
/// </summary>
public enum SyncCategory
{
    /// <summary>Already synced and not modified after the cut-off — nothing to do.</summary>
    NotRequired,

    /// <summary>Successfully synced before, but modified after the cut-off — needs re-sync.</summary>
    ModificationToSync,

    /// <summary>Not synced and not modified after the cut-off — failed during last sync.</summary>
    FailedDuringLastSync,

    /// <summary>Not synced and modified after the cut-off — first-time sync required.</summary>
    FirstTimeSync
}

public static class SyncCategoryMessages
{
    public static string GetMessage(SyncCategory category) => category switch
    {
        SyncCategory.NotRequired => "Not Required",
        SyncCategory.ModificationToSync => "Synced successfully during last sync. Now modification to be synced.",
        SyncCategory.FailedDuringLastSync => "Failed to sync during last sync.",
        SyncCategory.FirstTimeSync => "First time sync.",
        _ => "Unknown"
    };

    public static SyncCategory Classify(bool isSynced, DateTime lastModifiedDate, DateTime lastUpdatedDate)
    {
        // Normalise to date-only for comparison (time component ignored)
        var modDate = lastModifiedDate.Date;
        var cutOff = lastUpdatedDate.Date;

        return (isSynced, modDate > cutOff) switch
        {
            (true, false) => SyncCategory.NotRequired,
            (true, true) => SyncCategory.ModificationToSync,
            (false, false) => SyncCategory.FailedDuringLastSync,
            (false, true) => SyncCategory.FirstTimeSync
        };
    }
}

// ─── SyncHistory entity ───────────────────────────────────────────────────────

public class SyncHistory
{
    public int SyncHistoryId { get; set; }
    public DateTime SyncDate { get; set; } = DateTime.UtcNow;

    /// <summary>Vendor / Product / SupplyOrder.</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>See <see cref="SyncStatus"/>.</summary>
    public string Status { get; set; } = SyncStatus.Pending;

    public int RecordsProcessed { get; set; }

    /// <summary>The last_updated_date value sent to the DPSU portal for this sync run.</summary>
    public DateTime LastUpdatedDate { get; set; }

    public string? ErrorDescription { get; set; }
    public string? ResponseData { get; set; }

    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}