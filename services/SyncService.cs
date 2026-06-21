using AutoMapper;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

/// <summary>
/// Represents the outbound call to the actual SrijanDEEP government portal API.
/// The real base URL / auth scheme / payload contract were not part of the
/// requirements supplied, so this is a clean seam: swap StubSyncProviderClient
/// for a real HttpClient-based implementation once the portal's API contract
/// is available, without touching SyncService at all.
/// </summary>
public interface ISyncProviderClient
{
    Task<(bool Success, int RecordsProcessed, string? ResponseJson, string? Error)> PushEntityDataAsync(string entityType);
}

public class StubSyncProviderClient : ISyncProviderClient
{
    public Task<(bool Success, int RecordsProcessed, string? ResponseJson, string? Error)> PushEntityDataAsync(string entityType)
    {
        // Placeholder implementation. Replace with a real HttpClient call to the
        // SrijanDEEP portal's sync endpoint (configured via SyncProviderSettings).
        return Task.FromResult<(bool, int, string?, string?)>(
            (true, 0, "{\"status\":\"stub-not-implemented\"}", null));
    }
}

public interface ISyncService
{
    Task<PagedResult<SyncHistoryResponseDto>> GetSyncStatusListAsync(SyncHistoryFilterParams filter);
    Task<SyncHistoryDetailDto?> GetSyncStatusDetailAsync(int syncHistoryId);
    Task<SyncHistoryDetailDto?> GetErrorDescriptionAsync(int syncHistoryId);
    Task<(bool Success, string Message, SyncHistoryResponseDto? Data)> TriggerManualSyncAsync(TriggerSyncRequestDto request);
}

public class SyncService : ISyncService
{
    private static readonly string[] ValidEntityTypes =
    {
        SyncEntityType.Vendor, SyncEntityType.Product, SyncEntityType.SupplyOrder
    };

    private readonly ISyncHistoryRepository _syncHistoryRepository;
    private readonly ISyncProviderClient _syncProviderClient;
    private readonly IMapper _mapper;

    public SyncService(
        ISyncHistoryRepository syncHistoryRepository,
        ISyncProviderClient syncProviderClient,
        IMapper mapper)
    {
        _syncHistoryRepository = syncHistoryRepository;
        _syncProviderClient = syncProviderClient;
        _mapper = mapper;
    }

    public async Task<PagedResult<SyncHistoryResponseDto>> GetSyncStatusListAsync(SyncHistoryFilterParams filter)
    {
        var (items, totalCount) = await _syncHistoryRepository.GetPagedAsync(filter);

        return new PagedResult<SyncHistoryResponseDto>
        {
            Items = _mapper.Map<List<SyncHistoryResponseDto>>(items),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalRecords = totalCount
        };
    }

    public async Task<SyncHistoryDetailDto?> GetSyncStatusDetailAsync(int syncHistoryId)
    {
        var record = await _syncHistoryRepository.GetByIdAsync(syncHistoryId);
        return record is null ? null : _mapper.Map<SyncHistoryDetailDto>(record);
    }

    public async Task<SyncHistoryDetailDto?> GetErrorDescriptionAsync(int syncHistoryId)
    {
        var record = await _syncHistoryRepository.GetByIdAsync(syncHistoryId);
        if (record is null) return null;

        // Same shape as the detail endpoint, but callers hitting this endpoint are
        // specifically interested in ErrorDescription/ResponseData for a failed sync.
        return _mapper.Map<SyncHistoryDetailDto>(record);
    }

    public async Task<(bool Success, string Message, SyncHistoryResponseDto? Data)> TriggerManualSyncAsync(TriggerSyncRequestDto request)
    {
        if (!ValidEntityTypes.Contains(request.EntityType, StringComparer.OrdinalIgnoreCase))
        {
            return (false, $"EntityType must be one of: {string.Join(", ", ValidEntityTypes)}.", null);
        }

        var syncRecord = new SyncHistory
        {
            SyncDate = DateTime.UtcNow,
            EntityType = request.EntityType,
            Status = SyncStatus.InProgress,
            RecordsProcessed = 0
        };

        await _syncHistoryRepository.AddAsync(syncRecord);
        await _syncHistoryRepository.SaveChangesAsync();

        var (success, recordsProcessed, responseJson, error) =
            await _syncProviderClient.PushEntityDataAsync(request.EntityType);

        syncRecord.Status = success ? SyncStatus.Success : SyncStatus.Failed;
        syncRecord.RecordsProcessed = recordsProcessed;
        syncRecord.ResponseData = responseJson;
        syncRecord.ErrorDescription = error;

        _syncHistoryRepository.Update(syncRecord);
        await _syncHistoryRepository.SaveChangesAsync();

        var message = success ? "Sync triggered and completed successfully." : "Sync triggered but failed. See error details.";
        return (true, message, _mapper.Map<SyncHistoryResponseDto>(syncRecord));
    }
}