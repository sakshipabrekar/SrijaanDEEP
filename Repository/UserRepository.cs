using Microsoft.EntityFrameworkCore;
using SrijanDEEP.API.Data;
using SrijanDEEP.API.Entities;

namespace SrijanDEEP.API.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
        => await DbSet.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetByEmailAsync(string email)
        => await DbSet.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> UsernameExistsAsync(string username)
        => await DbSet.AnyAsync(u => u.Username == username);

    public async Task<bool> EmailExistsAsync(string email)
        => await DbSet.AnyAsync(u => u.Email == email);
}