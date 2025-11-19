using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementação do repositório para ASN (Advance Shipping Notice)
    /// </summary>
    public class ASNRepository : BaseRepository<ASN>, IASNRepository
    {
        public ASNRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ASN?> GetByAsnNumberAsync(string asnNumber, int warehouseId)
        {
            return await _context.Set<ASN>()
                .FirstOrDefaultAsync(a => a.AsnNumber == asnNumber && a.WarehouseId == warehouseId && !a.IsDeleted);
        }

        public async Task<IEnumerable<ASN>> GetPendingAsync(int warehouseId)
        {
            var pendingStatuses = new[] { 1, 2, 3, 4 }; // Scheduled, InTransit, Arrived, Unloading
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId && pendingStatuses.Contains((int)a.Status) && !a.IsDeleted)
                .OrderBy(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetByStatusAsync(int warehouseId, int status)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId && (int)a.Status == status && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetScheduledForDateAsync(int warehouseId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId &&
                            a.ScheduledArrivalDate >= startDate && a.ScheduledArrivalDate < endDate &&
                            !a.IsDeleted)
                .OrderBy(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetOverdueAsync(int warehouseId)
        {
            var pendingStatuses = new[] { 1, 2, 3, 4 }; // Não recebidas
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId &&
                            a.ScheduledArrivalDate < DateTime.UtcNow &&
                            pendingStatuses.Contains((int)a.Status) &&
                            !a.IsDeleted)
                .OrderBy(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId &&
                            a.ScheduledArrivalDate >= from && a.ScheduledArrivalDate <= to &&
                            !a.IsDeleted)
                .OrderBy(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetBySupplierAsync(int warehouseId, int supplierId)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId && a.SupplierId == supplierId && !a.IsDeleted)
                .OrderByDescending(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetByPriorityAsync(int warehouseId, int priority)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId && (int)a.Priority == priority && !a.IsDeleted)
                .OrderBy(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetCriticalAsync(int warehouseId)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId &&
                            ((int)a.Priority == 3 || (int)a.Priority == 4) && // High, Critical
                            !a.IsDeleted)
                .OrderBy(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetInspectedAsync(int warehouseId)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId && a.IsInspected && !a.IsDeleted)
                .OrderByDescending(a => a.InspectionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetPendingInspectionAsync(int warehouseId)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId &&
                            ((int)a.Status == 3 || (int)a.Status == 4) && // Arrived, Unloading
                            !a.IsInspected && !a.IsDeleted)
                .OrderBy(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ASN>> GetRejectedAsync(int warehouseId)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId &&
                            ((int)a.Status == 6 || (int)a.Status == 7) && // Rejected, Cancelled
                            !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<ASN?> GetByExternalReferenceAsync(int warehouseId, string externalReference)
        {
            return await _context.Set<ASN>()
                .FirstOrDefaultAsync(a => a.WarehouseId == warehouseId &&
                                          a.ExternalReference == externalReference &&
                                          !a.IsDeleted);
        }

        public async Task<int> CountPendingAsync(int warehouseId)
        {
            var pendingStatuses = new[] { 1, 2, 3, 4 };
            return await _context.Set<ASN>()
                .CountAsync(a => a.WarehouseId == warehouseId && pendingStatuses.Contains((int)a.Status) && !a.IsDeleted);
        }

        public async Task<int> CountOverdueAsync(int warehouseId)
        {
            var pendingStatuses = new[] { 1, 2, 3, 4 };
            return await _context.Set<ASN>()
                .CountAsync(a => a.WarehouseId == warehouseId &&
                                 a.ScheduledArrivalDate < DateTime.UtcNow &&
                                 pendingStatuses.Contains((int)a.Status) &&
                                 !a.IsDeleted);
        }

        public async Task<IEnumerable<ASN>> GetWithDiscrepanciesAsync(int warehouseId)
        {
            return await _context.Set<ASN>()
                .Include(a => a.Items)
                .Where(a => a.WarehouseId == warehouseId &&
                            a.Items.Any(i => i.ReceivedQuantity != i.ExpectedQuantity) &&
                            !a.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid asnId, int newStatus)
        {
            var asn = await GetByIdAsync(asnId);
            if (asn == null)
                return false;

            asn.Status = (ASNStatus)newStatus;
            return true;
        }

        public async Task<ASN?> GetWithItemsAsync(Guid asnId)
        {
            return await _context.Set<ASN>()
                .Include(a => a.Items)
                .FirstOrDefaultAsync(a => a.Id == asnId && !a.IsDeleted);
        }

        public async Task<IEnumerable<ASN>> GetInternalTransfersAsync(int warehouseId)
        {
            return await _context.Set<ASN>()
                .Where(a => a.WarehouseId == warehouseId && a.OriginWarehouseId.HasValue && !a.IsDeleted)
                .OrderByDescending(a => a.ScheduledArrivalDate)
                .ToListAsync();
        }
    }
}
