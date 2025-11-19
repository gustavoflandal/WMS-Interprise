using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementação do repositório para Localizações de Armazenamento
    /// </summary>
    public class StorageLocationRepository : BaseRepository<StorageLocation>, IStorageLocationRepository
    {
        public StorageLocationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<StorageLocation?> GetByCodeAsync(string code, int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .FirstOrDefaultAsync(l => l.Code == code && l.WarehouseId == warehouseId && !l.IsDeleted);
        }

        public async Task<IEnumerable<StorageLocation>> GetAvailableAsync(int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId &&
                            (l.Status == LocationStatus.Available ||
                             l.Status == LocationStatus.PartiallyOccupied) &&
                            !l.IsBlocked && !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<StorageLocation>> GetByZoneAsync(int warehouseId, int zone)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId && (int)l.Zone == zone && !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<StorageLocation>> GetOccupiedAsync(int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId &&
                            (l.Status == LocationStatus.Occupied ||
                             l.Status == LocationStatus.PartiallyOccupied) &&
                            !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<StorageLocation>> GetPartiallyOccupiedAsync(int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId &&
                            l.Status == LocationStatus.PartiallyOccupied &&
                            !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<StorageLocation>> GetUnavailableAsync(int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId &&
                            (l.Status == LocationStatus.Unavailable || l.IsBlocked) &&
                            !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<StorageLocation>> GetTemperatureControlledAsync(int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId && l.RequiresTemperatureControl && !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<StorageLocation>> GetByCorridorAsync(int warehouseId, string corridorNumber)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId && l.CorridorNumber == corridorNumber && !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<StorageLocation>> GetNearestToConsolidationAsync(int warehouseId, int limit)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId &&
                            (l.Status == LocationStatus.Available ||
                             l.Status == LocationStatus.PartiallyOccupied) &&
                            !l.IsBlocked && !l.IsDeleted)
                .OrderBy(l => l.DistanceFromConsolidationPoint)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<bool> CanAccommodateProductAsync(Guid locationId, int productId, int quantity)
        {
            var location = await GetByIdAsync(locationId);
            if (location == null || location.IsBlocked || location.Status == LocationStatus.Unavailable)
                return false;

            // Verificar se a localização está vazia ou contém o mesmo produto
            if (location.CurrentProductId != null && location.CurrentProductId != productId)
                return false;

            // Verificar capacidade
            return location.CurrentQuantity + quantity <= location.MaxCapacityUnits;
        }

        public async Task<IEnumerable<StorageLocation>> GetByProductAsync(int productId, int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId && l.CurrentProductId == productId && !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<IEnumerable<StorageLocation>> GetWithCapacityAsync(int warehouseId, int requiredCapacity)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId &&
                            (l.Status == LocationStatus.Available ||
                             l.Status == LocationStatus.PartiallyOccupied) &&
                            !l.IsBlocked && !l.IsDeleted &&
                            (l.MaxCapacityUnits - l.CurrentQuantity) >= requiredCapacity)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }

        public async Task<StorageLocation?> GetNextAvailableAsync(int warehouseId, int zone)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId && (int)l.Zone == zone &&
                            (l.Status == LocationStatus.Available ||
                             l.Status == LocationStatus.PartiallyOccupied) &&
                            !l.IsBlocked && !l.IsDeleted)
                .OrderBy(l => l.Code)
                .FirstOrDefaultAsync();
        }

        public async Task<int> CountAvailableAsync(int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .CountAsync(l => l.WarehouseId == warehouseId &&
                                 (l.Status == LocationStatus.Available ||
                                  l.Status == LocationStatus.PartiallyOccupied) &&
                                 !l.IsBlocked && !l.IsDeleted);
        }

        public async Task<int> CountOccupiedAsync(int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .CountAsync(l => l.WarehouseId == warehouseId &&
                                 (l.Status == LocationStatus.Occupied ||
                                  l.Status == LocationStatus.PartiallyOccupied) &&
                                 !l.IsDeleted);
        }

        public async Task<decimal> GetUtilizationPercentageAsync(int warehouseId)
        {
            var total = await _context.Set<StorageLocation>()
                .CountAsync(l => l.WarehouseId == warehouseId && !l.IsDeleted && !l.IsBlocked);

            var occupied = await CountOccupiedAsync(warehouseId);

            return total == 0 ? 0 : (decimal)occupied / total * 100;
        }

        public async Task<bool> UpdateStatusAsync(Guid locationId, int newStatus)
        {
            var location = await GetByIdAsync(locationId);
            if (location == null)
                return false;

            location.Status = (LocationStatus)newStatus;
            return true;
        }

        public async Task<bool> BlockLocationAsync(Guid locationId, string reason)
        {
            var location = await GetByIdAsync(locationId);
            if (location == null)
                return false;

            location.IsBlocked = true;
            location.BlockReason = reason;
            location.Status = LocationStatus.Unavailable;
            return true;
        }

        public async Task<bool> UnblockLocationAsync(Guid locationId)
        {
            var location = await GetByIdAsync(locationId);
            if (location == null)
                return false;

            location.IsBlocked = false;
            location.BlockReason = null;
            location.Status = LocationStatus.Available;
            return true;
        }

        public async Task<bool> RecordAccessAsync(Guid locationId)
        {
            var location = await GetByIdAsync(locationId);
            if (location == null)
                return false;

            location.AccessCount++;
            location.LastRemovalDate = DateTime.UtcNow;
            return true;
        }

        public async Task<IEnumerable<StorageLocation>> GetOrphanedAsync(int warehouseId)
        {
            return await _context.Set<StorageLocation>()
                .Where(l => l.WarehouseId == warehouseId &&
                            l.CurrentQuantity > 0 && l.CurrentProductId == null &&
                            !l.IsDeleted)
                .OrderBy(l => l.Code)
                .ToListAsync();
        }
    }
}
