using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Services;

// ─── Per-record result returned by the DPSU portal ───────────────────────────

/// <summary>
/// Represents one record returned by the DPSU portal fetch-data endpoint.
/// Map the actual portal JSON fields onto this shape in DpsuSyncProviderClient.
/// </summary>
public class DpsuRecord
{
    public string RecordId { get; set; } = string.Empty;
    public bool IsSynced { get; set; }
    public DateTime LastModifiedDate { get; set; }
}

// ─── Portal client contract ───────────────────────────────────────────────────

/// <summary>
/// Calls the DPSU portal endpoint:
///   GET https://[DPSU-API-URL]/fetch-data?data_type={entityType}&last_updated_date={dd-MM-yyyy}
/// </summary>
public interface ISyncProviderClient
{
    /// <param name="entityType">product / vendor / supplyorder (lower-cased by the caller)</param>
    /// <param name="lastUpdatedDate">Cut-off date passed as last_updated_date query param.</param>
    Task<(bool Success, List<DpsuRecord> Records, string? ResponseJson, string? Error)>
        FetchDataAsync(string entityType, DateTime lastUpdatedDate);
}

// ─── Stub implementation (swap for real HttpClient once portal contract is known) ──

public class StubSyncProviderClient : ISyncProviderClient
{
    public Task<(bool Success, List<DpsuRecord> Records, string? ResponseJson, string? Error)>
        FetchDataAsync(string entityType, DateTime lastUpdatedDate)
    {
        // TODO: Replace with real HttpClient call:
        //   GET https://[DPSU-API-URL]/fetch-data
        //       ?data_type={entityType.ToLower()}
        //       &last_updated_date={lastUpdatedDate:dd-MM-yyyy}
        //
        // Parse the portal's JSON response into List<DpsuRecord>.

        var stubRecords = new List<DpsuRecord>
        {
            // Example stub records covering all four classification cases:
            new() { RecordId = "REC-001", IsSynced = true,  LastModifiedDate = lastUpdatedDate.AddDays(-5) }, // NotRequired
            new() { RecordId = "REC-002", IsSynced = true,  LastModifiedDate = lastUpdatedDate.AddDays(+2) }, // ModificationToSync
            new() { RecordId = "REC-003", IsSynced = false, LastModifiedDate = lastUpdatedDate.AddDays(-3) }, // FailedDuringLastSync
            new() { RecordId = "REC-004", IsSynced = false, LastModifiedDate = lastUpdatedDate.AddDays(+1) }, // FirstTimeSync
        };

        return Task.FromResult<(bool, List<DpsuRecord>, string?, string?)>(
            (true, stubRecords, "{\"status\":\"stub\"}", null));
    }
}