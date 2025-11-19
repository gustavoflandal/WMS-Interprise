using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementação do repositório para Produtos (SKUs)
    /// </summary>
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetBySkuAsync(string sku, int tenantId)
        {
            return await _context.Set<Product>()
                .FirstOrDefaultAsync(p => p.Sku == sku && p.TenantId == tenantId && !p.IsDeleted);
        }

        public async Task<bool> SkuExistsAsync(string sku, int tenantId)
        {
            return await _context.Set<Product>()
                .AnyAsync(p => p.Sku == sku && p.TenantId == tenantId && !p.IsDeleted);
        }

        public async Task<IEnumerable<Product>> GetActiveAsync(int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId, int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => (int)p.Category == categoryId && p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByABCClassificationAsync(int classification, int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => p.ABCClassification.HasValue && (int)p.ABCClassification == classification &&
                            p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetRequiringLotTrackingAsync(int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => p.RequiresLotTracking && p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetRequiringSerialNumberAsync(int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => p.RequiresSerialNumber && p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPerishableAsync(int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => p.ShelfLifeDays.HasValue && p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string searchTerm, int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => (p.Sku.Contains(searchTerm) || p.Name.Contains(searchTerm)) &&
                            p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetRequiringTemperatureControlAsync(int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => (p.MinStorageTemperature.HasValue || p.MaxStorageTemperature.HasValue) &&
                            p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetDangerousAsync(int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => (p.IsFlammable || p.IsDangerous) &&
                            p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPharmaceuticalAsync(int tenantId)
        {
            return await _context.Set<Product>()
                .Where(p => p.IsPharmaceutical && p.TenantId == tenantId && p.IsActive && !p.IsDeleted)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}
