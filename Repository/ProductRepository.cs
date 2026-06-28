using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetByIdAsync(int productId);
    Task<Product?> GetByUrnAsync(string urn);
    Task<(List<Product> Items, int TotalCount)> GetPagedAsync(ProductFilterParams filter);
}

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Product?> GetByIdAsync(int productId)
        => await DbSet.Include(p => p.Vendor).FirstOrDefaultAsync(p => p.Id == productId);

    public async Task<Product?> GetByUrnAsync(string urn)
        => await DbSet.Include(p => p.Vendor).FirstOrDefaultAsync(p => p.URN_No == urn);

    public async Task<(List<Product> Items, int TotalCount)> GetPagedAsync(ProductFilterParams filter)
    {
        IQueryable<Product> query = DbSet.AsNoTracking().Include(p => p.Vendor);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(p =>
                p.Product_Name.Contains(term) ||
                p.URN_No.Contains(term) ||
                (p.Part_No != null && p.Part_No.Contains(term)) ||
                (p.Defence_Platform != null && p.Defence_Platform.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(filter.URN_No))
            query = query.Where(p => p.URN_No == filter.URN_No);

        if (!string.IsNullOrWhiteSpace(filter.Defence_Platform))
            query = query.Where(p => p.Defence_Platform == filter.Defence_Platform);

        if (!string.IsNullOrWhiteSpace(filter.Product_Type))
            query = query.Where(p => p.Product_Type == filter.Product_Type);

        if (filter.Is_Item_Supplied.HasValue)
            query = query.Where(p => p.Is_Item_Supplied == filter.Is_Item_Supplied.Value);

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
            "product_name" => descending ? query.OrderByDescending(p => p.Product_Name) : query.OrderBy(p => p.Product_Name),
            "uploaded_datetime" => descending ? query.OrderByDescending(p => p.Uploaded_DateTime) : query.OrderBy(p => p.Uploaded_DateTime),
            "last_modified_datetime" => descending ? query.OrderByDescending(p => p.Last_Modified_DateTime) : query.OrderBy(p => p.Last_Modified_DateTime),
            _ => descending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id)
        };
    }
}