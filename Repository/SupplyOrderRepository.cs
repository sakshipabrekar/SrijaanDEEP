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
        => await DbSet.Include(s => s.Product).FirstOrDefaultAsync(s => s.SupplyOrderId == supplyOrderId);

    public async Task<(List<SupplyOrder> Items, int TotalCount)> GetPagedAsync(SupplyOrderFilterParams filter)
    {
        IQueryable<SupplyOrder> query = DbSet.AsNoTracking().Include(s => s.Product);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(s =>
                s.PurchaseOrderNumber.Contains(term) ||
                (s.VendorName != null && s.VendorName.Contains(term)) ||
                (s.URNNumber != null && s.URNNumber.Contains(term)));
        }

        if (filter.ProductId.HasValue)
            query = query.Where(s => s.ProductId == filter.ProductId.Value);

        if (filter.IsVendorMSME.HasValue)
            query = query.Where(s => s.IsVendorMSME == filter.IsVendorMSME.Value);

        if (filter.PurchaseOrderDateFrom.HasValue)
            query = query.Where(s => s.PurchaseOrderDate >= filter.PurchaseOrderDateFrom.Value);

        if (filter.PurchaseOrderDateTo.HasValue)
            query = query.Where(s => s.PurchaseOrderDate <= filter.PurchaseOrderDateTo.Value);

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
            "purchaseorderdate" => descending ? query.OrderByDescending(s => s.PurchaseOrderDate) : query.OrderBy(s => s.PurchaseOrderDate),
            "totallinevalue" => descending ? query.OrderByDescending(s => s.TotalLineValue) : query.OrderBy(s => s.TotalLineValue),
            "createddate" => descending ? query.OrderByDescending(s => s.CreatedDate) : query.OrderBy(s => s.CreatedDate),
            "lastmodifieddate" => descending ? query.OrderByDescending(s => s.LastModifiedDate) : query.OrderBy(s => s.LastModifiedDate),
            _ => descending ? query.OrderByDescending(s => s.SupplyOrderId) : query.OrderBy(s => s.SupplyOrderId)
        };
    }
}