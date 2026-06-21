using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
        => await DbSet.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.Token == token);
}