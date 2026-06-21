using SrijanDEEP.API.Common;

namespace SrijanDEEP.API.DTOs;

public class UploadResultDto
{
    public int UploadHistoryId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string UploadType { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int FailedRecords { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> RowErrors { get; set; } = new();
}

public class UploadHistoryResponseDto
{
    public int UploadHistoryId { get; set; }
    public string UploadType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int FailedRecords { get; set; }
    public int UploadedBy { get; set; }
    public string? UploadedByUsername { get; set; }
    public DateTime UploadDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class UploadHistoryFilterParams : PaginationParams
{
    public string? EntityType { get; set; }
    public string? Status { get; set; }
    public DateTime? UploadDateFrom { get; set; }
    public DateTime? UploadDateTo { get; set; }
}