using Microsoft.EntityFrameworkCore;
using srijaanDEEP.Models;
using srijaanDEEP.Repository;
using SrijanDEEP.API.Data;

namespace srijaanDEEP.Repository
{
    // ─── Interface ───────────────────────────────────────────────────────────────
    public interface IProductTypeRepository
    {
        Task<IEnumerable<ProductTypeMaster>> GetAllAsync();
        Task<ProductTypeMaster> GetByIdAsync(int id);
        Task<ProductTypeMaster> CreateAsync(ProductTypeMaster entity);
        Task<ProductTypeMaster> UpdateAsync(ProductTypeMaster entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}



    public class ProductTypeRepository : IProductTypeRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductTypeMaster>> GetAllAsync()
            => await _context.ProductTypeMasters.ToListAsync();

        public async Task<ProductTypeMaster> GetByIdAsync(int id)
            => await _context.ProductTypeMasters.FindAsync(id);

        public async Task<ProductTypeMaster> CreateAsync(ProductTypeMaster entity)
        {
            _context.ProductTypeMasters.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ProductTypeMaster> UpdateAsync(ProductTypeMaster entity)
        {
            _context.ProductTypeMasters.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.ProductTypeMasters.FindAsync(id);
            if (entity == null) return false;
            _context.ProductTypeMasters.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.ProductTypeMasters.AnyAsync(e => e.Id == id);
    }
