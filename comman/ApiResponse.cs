namespace SrijanDEEP.API.Common;

/// <summary>
/// Standard response envelope returned by every endpoint:
/// { "success": true, "message": "...", "data": {} }
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    public static ApiResponse<T> SuccessResponse(T? data, string message = "Request processed successfully")
        => new() { Success = true, Message = message, Data = data };

    public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null)
        => new() { Success = false, Message = message, Data = default, Errors = errors };
}

/// <summary>Non-generic convenience version for endpoints with no payload.</summary>
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse<object> Ok(string message = "Request processed successfully")
        => SuccessResponse(null, message);

    public static ApiResponse<object> Fail(string message, List<string>? errors = null)
        => FailureResponse(message, errors);
}

/// <summary>Wrapper for any paginated GET result.</summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalRecords / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// Common query parameters accepted by all list/GET endpoints:
/// paging + free-text search + sorting.
/// </summary>
public class PaginationParams
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 1 : value);
    }

    /// <summary>Free-text search across the entity's key searchable fields.</summary>
    public string? SearchTerm { get; set; }

    /// <summary>Column name to sort by (entity-specific, validated server-side).</summary>
    public string? SortBy { get; set; }

    /// <summary>true = descending.</summary>
    public bool SortDescending { get; set; } = false;
}