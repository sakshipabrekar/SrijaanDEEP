using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface IVendorRepository : IGenericRepository<Vendor>
{
    Task<Vendor?> GetByURNNoAsync(string urnNo);
    Task<(List<Vendor> Items, int TotalCount)> GetPagedAsync(VendorFilterParams filter);
}

public class VendorRepository : GenericRepository<Vendor>, IVendorRepository
{
    public VendorRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Vendor?> GetByURNNoAsync(string urnNo)
        => await DbSet.FirstOrDefaultAsync(v => v.URN_No == urnNo);

    public async Task<(List<Vendor> Items, int TotalCount)> GetPagedAsync(VendorFilterParams filter)
    {
        IQueryable<Vendor> query = DbSet.AsNoTracking();

        // Free-text search across key identifiable fields
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(v =>
                v.Vendor_Org_Name.Contains(term) ||
                (v.GST_No != null && v.GST_No.Contains(term)) ||
                (v.PAN_No != null && v.PAN_No.Contains(term)) ||
                (v.URN_No.Contains(term)) ||
                (v.Nodal_Officer_Name != null && v.Nodal_Officer_Name.Contains(term)));
        }

        // Exact-match filters
        if (!string.IsNullOrWhiteSpace(filter.URN_No))
            query = query.Where(v => v.URN_No == filter.URN_No);

        if (!string.IsNullOrWhiteSpace(filter.GST_No))
            query = query.Where(v => v.GST_No == filter.GST_No);

        if (!string.IsNullOrWhiteSpace(filter.PAN_No))
            query = query.Where(v => v.PAN_No == filter.PAN_No);

        if (!string.IsNullOrWhiteSpace(filter.MSME_No))
            query = query.Where(v => v.MSME_No == filter.MSME_No);

        if (filter.IsActive.HasValue)
            query = query.Where(v => v.IsActive == filter.IsActive.Value);

        query = ApplySorting(query, filter.SortBy, filter.SortDescending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    private static IQueryable<Vendor> ApplySorting(IQueryable<Vendor> query, string? sortBy, bool descending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "vendor_org_name" => descending ? query.OrderByDescending(v => v.Vendor_Org_Name) : query.OrderBy(v => v.Vendor_Org_Name),
            "uploaded_datetime" => descending ? query.OrderByDescending(v => v.Uploaded_DateTime) : query.OrderBy(v => v.Uploaded_DateTime),
            "last_modified_datetime" => descending ? query.OrderByDescending(v => v.Last_Modified_DateTime) : query.OrderBy(v => v.Last_Modified_DateTime),
            _ => descending ? query.OrderByDescending(v => v.URN_No) : query.OrderBy(v => v.URN_No)
        };
    }
}