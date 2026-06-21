using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Data;

namespace SrijanDEEP.API.Repositories;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
    Task<IQueryable<T>> GetQueryableAsync(params Expression<Func<T, object>>[] includes);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task SaveChangesAsync();
}

/// <summary>
/// Base repository implementing the common CRUD-minus-Delete operations.
/// Entity-specific repositories extend this with their own search/filter logic.
/// </summary>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<T> DbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = DbSet;
        foreach (var include in includes)
            query = query.Include(include);

        // Assumes every entity exposes an Id property named "<Entity>Id" via reflection-free
        // approach is not possible generically without a common interface, so concrete
        // repositories override GetByIdAsync where a strongly-typed key lookup is needed.
        return await FindByPrimaryKeyAsync(id, query);
    }

    private async Task<T?> FindByPrimaryKeyAsync(int id, IQueryable<T> query)
    {
        var keyProperty = Context.Model.FindEntityType(typeof(T))!.FindPrimaryKey()!.Properties[0].Name;
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, keyProperty);
        var constant = Expression.Constant(id);
        var equality = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);

        return await query.FirstOrDefaultAsync(lambda);
    }

    public Task<IQueryable<T>> GetQueryableAsync(params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = DbSet;
        foreach (var include in includes)
            query = query.Include(include);

        return Task.FromResult(query);
    }

    public async Task<T> AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
        return entity;
    }

    public void Update(T entity)
    {
        DbSet.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        => await DbSet.AnyAsync(predicate);

    public async Task SaveChangesAsync() => await Context.SaveChangesAsync();
}