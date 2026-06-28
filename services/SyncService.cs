using AutoMapper;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface ISyncService
{
    Task<PagedResult<SyncHistoryResponseDto>> GetSyncStatusListAsync(SyncHistoryFilterParams filter);
    Task<SyncHistoryDetailDto?> GetSyncStatusDetailAsync(int syncHistoryId);
    Task<SyncHistoryDetailDto?> GetErrorDescriptionAsync(int syncHistoryId);
    Task<(bool Success, string Message, TriggerSyncResponseDto? Data)> TriggerManualSyncAsync(TriggerSyncRequestDto request);
}

public class SyncService : ISyncService
{
    private static readonly string[] ValidEntityTypes =
    {
        SyncEntityType.Vendor,
        SyncEntityType.Product,
        SyncEntityType.SupplyOrder
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

    // ─── GET paged list ───────────────────────────────────────────────────────

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

    // ─── GET single detail ────────────────────────────────────────────────────

    public async Task<SyncHistoryDetailDto?> GetSyncStatusDetailAsync(int syncHistoryId)
    {
        var record = await _syncHistoryRepository.GetByIdAsync(syncHistoryId);
        return record is null ? null : _mapper.Map<SyncHistoryDetailDto>(record);
    }

    // ─── GET error description ────────────────────────────────────────────────

    public async Task<SyncHistoryDetailDto?> GetErrorDescriptionAsync(int syncHistoryId)
    {
        var record = await _syncHistoryRepository.GetByIdAsync(syncHistoryId);
        return record is null ? null : _mapper.Map<SyncHistoryDetailDto>(record);
    }

    // ─── POST trigger manual sync ─────────────────────────────────────────────

    public async Task<(bool Success, string Message, TriggerSyncResponseDto? Data)>
        TriggerManualSyncAsync(TriggerSyncRequestDto request)
    {
        // 1. Validate entity type
        if (!ValidEntityTypes.Contains(request.EntityType, StringComparer.OrdinalIgnoreCase))
        {
            return (false, $"EntityType must be one of: {string.Join(", ", ValidEntityTypes)}.", null);
        }

        // 2. Create an in-progress history record
        var syncRecord = new SyncHistory
        {
            SyncDate = DateTime.UtcNow,
            EntityType = request.EntityType,
            Status = SyncStatus.InProgress,
            LastUpdatedDate = request.LastUpdatedDate,
            RecordsProcessed = 0
        };

        await _syncHistoryRepository.AddAsync(syncRecord);
        await _syncHistoryRepository.SaveChangesAsync();

        // 3. Call the DPSU portal
        //    URL pattern: GET https://[DPSU-API-URL]/fetch-data
        //                     ?data_type={entityType.ToLower()}
        //                     &last_updated_date={dd-MM-yyyy}
        var (portalSuccess, dpsuRecords, responseJson, error) =
            await _syncProviderClient.FetchDataAsync(
                request.EntityType.ToLower(),
                request.LastUpdatedDate);

        // 4. Classify each record using the four-quadrant rule
        var recordResults = dpsuRecords.Select(rec =>
        {
            var category = SyncCategoryMessages.Classify(
                rec.IsSynced,
                rec.LastModifiedDate,
                request.LastUpdatedDate);

            return new SyncRecordResultDto
            {
                RecordId = rec.RecordId,
                IsSynced = rec.IsSynced,
                LastModifiedDate = rec.LastModifiedDate,
                SyncCategory = category.ToString(),
                SyncMessage = SyncCategoryMessages.GetMessage(category)
            };
        }).ToList();

        // 5. Determine how many records actually needed action
        int recordsProcessed = recordResults.Count(r =>
            r.SyncCategory is
                nameof(SyncCategory.ModificationToSync) or
                nameof(SyncCategory.FirstTimeSync));

        // 6. Overall status:
        //    - Portal call failed                     → Failed
        //    - Some records FailedDuringLastSync exist → PartialSuccess
        //    - All actionable records OK              → Success
        string overallStatus;
        if (!portalSuccess)
        {
            overallStatus = SyncStatus.Failed;
        }
        else if (recordResults.Any(r => r.SyncCategory == nameof(SyncCategory.FailedDuringLastSync)))
        {
            overallStatus = SyncStatus.PartialSuccess;
        }
        else
        {
            overallStatus = SyncStatus.Success;
        }

        // 7. Persist final state
        syncRecord.Status = overallStatus;
        syncRecord.RecordsProcessed = recordsProcessed;
        syncRecord.ResponseData = responseJson;
        syncRecord.ErrorDescription = error;
        syncRecord.LastModifiedDate = DateTime.UtcNow;

        _syncHistoryRepository.Update(syncRecord);
        await _syncHistoryRepository.SaveChangesAsync();

        // 8. Build response
        var responseDto = new TriggerSyncResponseDto
        {
            SyncHistoryId = syncRecord.SyncHistoryId,
            EntityType = syncRecord.EntityType,
            LastUpdatedDate = syncRecord.LastUpdatedDate,
            OverallStatus = overallStatus,
            TotalRecords = dpsuRecords.Count,
            RecordsProcessed = recordsProcessed,
            RecordResults = recordResults
        };

        string message = overallStatus switch
        {
            SyncStatus.Success => "Sync completed successfully.",
            SyncStatus.PartialSuccess => "Sync completed with some failures. See record results.",
            _ => "Sync failed. See error details."
        };

        return (true, message, responseDto);
    }
}