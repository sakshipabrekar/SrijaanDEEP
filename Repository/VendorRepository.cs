using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface IVendorRepository : IGenericRepository<Vendor>
{
    Task<Vendor?> GetByIdAsync(int vendorId);
    Task<Vendor?> GetByURNNumberAsync(string urnNumber);
    Task<(List<Vendor> Items, int TotalCount)> GetPagedAsync(VendorFilterParams filter);
}

public class VendorRepository : GenericRepository<Vendor>, IVendorRepository
{
    public VendorRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Vendor?> GetByIdAsync(int vendorId)
        => await DbSet.FirstOrDefaultAsync(v => v.VendorId == vendorId);

    public async Task<Vendor?> GetByURNNumberAsync(string urnNumber)
        => await DbSet.FirstOrDefaultAsync(v => v.URNNumber == urnNumber);

    public async Task<(List<Vendor> Items, int TotalCount)> GetPagedAsync(VendorFilterParams filter)
    {
        IQueryable<Vendor> query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(v =>
                v.VendorOrganisationName.Contains(term) ||
                (v.GSTNumber != null && v.GSTNumber.Contains(term)) ||
                (v.PANNumber != null && v.PANNumber.Contains(term)) ||
                (v.URNNumber != null && v.URNNumber.Contains(term)) ||
                (v.NodalOfficerName != null && v.NodalOfficerName.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(filter.GSTNumber))
            query = query.Where(v => v.GSTNumber == filter.GSTNumber);

        if (!string.IsNullOrWhiteSpace(filter.PANNumber))
            query = query.Where(v => v.PANNumber == filter.PANNumber);

        if (!string.IsNullOrWhiteSpace(filter.MSMENumber))
            query = query.Where(v => v.MSMENumber == filter.MSMENumber);

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
        // Kept as IQueryable (not AsEnumerable) so sorting + paging both translate to SQL.
        return sortBy?.ToLowerInvariant() switch
        {
            "vendororganisationname" => descending
                ? query.OrderByDescending(v => v.VendorOrganisationName)
                : query.OrderBy(v => v.VendorOrganisationName),
            "createddate" => descending
                ? query.OrderByDescending(v => v.CreatedDate)
                : query.OrderBy(v => v.CreatedDate),
            "lastmodifieddate" => descending
                ? query.OrderByDescending(v => v.LastModifiedDate)
                : query.OrderBy(v => v.LastModifiedDate),
            _ => descending
                ? query.OrderByDescending(v => v.VendorId)
                : query.OrderBy(v => v.VendorId)
        };
    }
}