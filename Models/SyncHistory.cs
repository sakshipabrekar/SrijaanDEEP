namespace SrijanDEEP.API.Entities;

public static class SyncEntityType
{
    public const string Vendor = "Vendor";
    public const string Product = "Product";
    public const string SupplyOrder = "SupplyOrder";
}

public static class SyncStatus
{
    public const string Pending = "Pending";
    public const string InProgress = "InProgress";
    public const string Success = "Success";
    public const string Failed = "Failed";
    public const string PartialSuccess = "PartialSuccess";
}

public class SyncHistory
{
    public int SyncHistoryId { get; set; }
    public DateTime SyncDate { get; set; } = DateTime.UtcNow;

    /// <summary>Vendor / Product / SupplyOrder - see <see cref="SyncEntityType"/>.</summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>See <see cref="SyncStatus"/>.</summary>
    public string Status { get; set; } = SyncStatus.Pending;

    public int RecordsProcessed { get; set; }
    public string? ErrorDescription { get; set; }
    public string? ResponseData { get; set; }

    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}