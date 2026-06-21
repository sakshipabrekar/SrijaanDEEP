using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface IUploadHistoryRepository : IGenericRepository<UploadHistory>
{
    Task<(List<UploadHistory> Items, int TotalCount)> GetPagedAsync(UploadHistoryFilterParams filter);
}

public class UploadHistoryRepository : GenericRepository<UploadHistory>, IUploadHistoryRepository
{
    public UploadHistoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(List<UploadHistory> Items, int TotalCount)> GetPagedAsync(UploadHistoryFilterParams filter)
    {
        IQueryable<UploadHistory> query = DbSet.AsNoTracking().Include(u => u.UploadedByUser);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(u => u.FileName.Contains(filter.SearchTerm));

        if (!string.IsNullOrWhiteSpace(filter.EntityType))
            query = query.Where(u => u.EntityType == filter.EntityType);

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(u => u.Status == filter.Status);

        if (filter.UploadDateFrom.HasValue)
            query = query.Where(u => u.UploadDate >= filter.UploadDateFrom.Value);

        if (filter.UploadDateTo.HasValue)
            query = query.Where(u => u.UploadDate <= filter.UploadDateTo.Value);

        query = filter.SortDescending
            ? query.OrderByDescending(u => u.UploadDate)
            : query.OrderBy(u => u.UploadDate);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}