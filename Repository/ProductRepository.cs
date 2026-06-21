using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetByIdAsync(int productId);
    Task<Product?> GetByUniqueReferenceNumberAsync(string urn);
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(ProductFilterParams filter);
}

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Product?> GetByIdAsync(int productId)
        => await DbSet.Include(p => p.Vendor).FirstOrDefaultAsync(p => p.ProductId == productId);

    public async Task<Product?> GetByUniqueReferenceNumberAsync(string urn)
        => await DbSet.FirstOrDefaultAsync(p => p.UniqueReferenceNumber == urn);

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(ProductFilterParams filter)
    {
        IQueryable<Product> query = DbSet.AsNoTracking().Include(p => p.Vendor);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(p =>
                p.ProductName.Contains(term) ||
                p.UniqueReferenceNumber.Contains(term) ||
                (p.CompanyName != null && p.CompanyName.Contains(term)) ||
                (p.PartNumber != null && p.PartNumber.Contains(term)) ||
                (p.DefencePlatform != null && p.DefencePlatform.Contains(term)));
        }

        if (filter.VendorId.HasValue)
            query = query.Where(p => p.VendorId == filter.VendorId.Value);

        if (!string.IsNullOrWhiteSpace(filter.DefencePlatform))
            query = query.Where(p => p.DefencePlatform == filter.DefencePlatform);

        if (!string.IsNullOrWhiteSpace(filter.ProductType))
            query = query.Where(p => p.ProductType == filter.ProductType);

        if (filter.IsItemSupplied.HasValue)
            query = query.Where(p => p.IsItemSupplied == filter.IsItemSupplied.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(p => p.IsActive == filter.IsActive.Value);

        query = ApplySorting(query, filter.SortBy, filter.SortDescending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, string? sortBy, bool descending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "productname" => descending ? query.OrderByDescending(p => p.ProductName) : query.OrderBy(p => p.ProductName),
            "createddate" => descending ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
            "lastmodifieddate" => descending ? query.OrderByDescending(p => p.LastModifiedDate) : query.OrderBy(p => p.LastModifiedDate),
            _ => descending ? query.OrderByDescending(p => p.ProductId) : query.OrderBy(p => p.ProductId)
        };
    }
}