using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface ISupplyOrderRepository : IGenericRepository<SupplyOrder>
{
    Task<SupplyOrder?> GetByIdAsync(int supplyOrderId);
    Task<(List<SupplyOrder> Items, int TotalCount)> GetPagedAsync(SupplyOrderFilterParams filter);
}

public class SupplyOrderRepository : GenericRepository<SupplyOrder>, ISupplyOrderRepository
{
    public SupplyOrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<SupplyOrder?> GetByIdAsync(int supplyOrderId)
        => await DbSet
            .Include(s => s.Product)
            .Include(s => s.Vendor)
            .FirstOrDefaultAsync(s => s.Id == supplyOrderId);

    public async Task<(List<SupplyOrder> Items, int TotalCount)> GetPagedAsync(SupplyOrderFilterParams filter)
    {
        IQueryable<SupplyOrder> query = DbSet.AsNoTracking()
            .Include(s => s.Product)
            .Include(s => s.Vendor);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(s =>
                s.PO_No.Contains(term) ||
                (s.Vendor != null && s.Vendor.Vendor_Org_Name.Contains(term)) ||
                s.URN_No.Contains(term));
        }

        if (filter.ProductId.HasValue)
            query = query.Where(s => s.ProductId == filter.ProductId.Value);

        if (!string.IsNullOrWhiteSpace(filter.URN_No))
            query = query.Where(s => s.URN_No == filter.URN_No);

        if (filter.Whether_MSME.HasValue)
            query = query.Where(s => s.Whether_MSME == filter.Whether_MSME.Value);

        if (filter.PO_DateFrom.HasValue)
            query = query.Where(s => s.PO_Date >= filter.PO_DateFrom.Value);

        if (filter.PO_DateTo.HasValue)
            query = query.Where(s => s.PO_Date <= filter.PO_DateTo.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(s => s.IsActive == filter.IsActive.Value);

        query = ApplySorting(query, filter.SortBy, filter.SortDescending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    private static IQueryable<SupplyOrder> ApplySorting(IQueryable<SupplyOrder> query, string? sortBy, bool descending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "po_date" => descending ? query.OrderByDescending(s => s.PO_Date) : query.OrderBy(s => s.PO_Date),
            "total_line_value" => descending ? query.OrderByDescending(s => s.Total_line_value) : query.OrderBy(s => s.Total_line_value),
            "uploaded_datetime" => descending ? query.OrderByDescending(s => s.Uploaded_DateTime) : query.OrderBy(s => s.Uploaded_DateTime),
            "last_modified_datetime" => descending ? query.OrderByDescending(s => s.Last_Modified_DateTime) : query.OrderBy(s => s.Last_Modified_DateTime),
            _ => descending ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id)
        };
    }
}