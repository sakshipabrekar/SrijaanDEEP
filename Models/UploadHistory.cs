namespace SrijanDEEP.API.Entities;

public static class UploadType
{
    public const string Manual = "Manual";
    public const string Bulk = "Bulk";
}

public static class UploadStatus
{
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string CompletedWithErrors = "CompletedWithErrors";
    public const string Failed = "Failed";
}

public class UploadHistory
{
    public int UploadHistoryId { get; set; }

    /// <summary>Manual / Bulk - see <see cref="UploadType"/>.</summary>
    public string UploadType { get; set; } = string.Empty;

    /// <summary>Vendor / Product / SupplyOrder.</summary>
    public string EntityType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int FailedRecords { get; set; }

    public int UploadedBy { get; set; }
    public User? UploadedByUser { get; set; }

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    /// <summary>See <see cref="UploadStatus"/>.</summary>
    public string Status { get; set; } = UploadStatus.InProgress;

    public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}