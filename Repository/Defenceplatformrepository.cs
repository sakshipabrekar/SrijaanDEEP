// ─── Implementation ──────────────────────────────────────────────────────────
using Microsoft.EntityFrameworkCore;
using srijaanDEEP.Models;
using srijaanDEEP.Repository;
using SrijanDEEP.API.Data;

namespace srijaanDEEP.Repository
{
    // ─── Interface ───────────────────────────────────────────────────────────────
    public interface IDefencePlatformRepository
    {
        Task<IEnumerable<DefencePlatformMaster>> GetAllAsync();
        Task<DefencePlatformMaster> GetByIdAsync(int id);
        Task<DefencePlatformMaster> CreateAsync(DefencePlatformMaster entity);
        Task<DefencePlatformMaster> UpdateAsync(DefencePlatformMaster entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}



    public class DefencePlatformRepository : IDefencePlatformRepository
    {
        private readonly ApplicationDbContext _context;

        public DefencePlatformRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DefencePlatformMaster>> GetAllAsync()
            => await _context.DefencePlatformMasters.ToListAsync();

        public async Task<DefencePlatformMaster> GetByIdAsync(int id)
            => await _context.DefencePlatformMasters.FindAsync(id);

        public async Task<DefencePlatformMaster> CreateAsync(DefencePlatformMaster entity)
        {
            _context.DefencePlatformMasters.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<DefencePlatformMaster> UpdateAsync(DefencePlatformMaster entity)
        {
            _context.DefencePlatformMasters.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.DefencePlatformMasters.FindAsync(id);
            if (entity == null) return false;
            _context.DefencePlatformMasters.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.DefencePlatformMasters.AnyAsync(e => e.Id == id);
    }
