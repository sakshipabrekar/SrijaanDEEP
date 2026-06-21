using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface ISyncHistoryRepository : IGenericRepository<SyncHistory>
{
    Task<SyncHistory?> GetByIdAsync(int syncHistoryId);
    Task<(List<SyncHistory> Items, int TotalCount)> GetPagedAsync(SyncHistoryFilterParams filter);
}

public class SyncHistoryRepository : GenericRepository<SyncHistory>, ISyncHistoryRepository
{
    public SyncHistoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<SyncHistory?> GetByIdAsync(int syncHistoryId)
        => await DbSet.FirstOrDefaultAsync(s => s.SyncHistoryId == syncHistoryId);

    public async Task<(List<SyncHistory> Items, int TotalCount)> GetPagedAsync(SyncHistoryFilterParams filter)
    {
        IQueryable<SyncHistory> query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.EntityType))
            query = query.Where(s => s.EntityType == filter.EntityType);

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(s => s.Status == filter.Status);

        if (filter.SyncDateFrom.HasValue)
            query = query.Where(s => s.SyncDate >= filter.SyncDateFrom.Value);

        if (filter.SyncDateTo.HasValue)
            query = query.Where(s => s.SyncDate <= filter.SyncDateTo.Value);

        query = filter.SortDescending
            ? query.OrderByDescending(s => s.SyncDate)
            : query.OrderBy(s => s.SyncDate);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}