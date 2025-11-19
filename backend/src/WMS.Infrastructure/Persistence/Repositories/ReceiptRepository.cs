using Microsoft.EntityFrameworkCore;
using WMS.Domain.Entities;
using WMS.Domain.Interfaces;

namespace WMS.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementação do repositório para Documentação de Recebimento
    /// </summary>
    public class ReceiptRepository : BaseRepository<ReceiptDocumentation>, IReceiptRepository
    {
        public ReceiptRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<ReceiptDocumentation?> GetByReceiptNumberAsync(string receiptNumber, int warehouseId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .FirstOrDefaultAsync(r => r.ReceiptNumber == receiptNumber && r.WarehouseId == warehouseId && !r.IsDeleted);
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetByDateAsync(int warehouseId, DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId &&
                            r.ReceiptDate >= startDate && r.ReceiptDate < endDate &&
                            !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetByDateRangeAsync(int warehouseId, DateTime from, DateTime to)
        {
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId &&
                            r.ReceiptDate >= from && r.ReceiptDate <= to &&
                            !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetByStatusAsync(int warehouseId, int status)
        {
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && (int)r.Status == status && !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetInProgressAsync(int warehouseId)
        {
            var inProgressStatuses = new[] { 2, 6 }; // InProgress, OnHold
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && inProgressStatuses.Contains((int)r.Status) && !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetPendingConfirmationAsync(int warehouseId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId &&
                            ((int)r.Status == 1 || (int)r.Status == 2) && // Draft, InProgress
                            !r.ConfirmedAt.HasValue && !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetWithDiscrepanciesAsync(int warehouseId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && r.HasDiscrepancies && !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetBySupplierAsync(int warehouseId, int supplierId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && r.SupplierId == supplierId && !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetByTypeAsync(int warehouseId, int receiptType)
        {
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && (int)r.ReceiptType == receiptType && !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<ReceiptDocumentation?> GetByAsnAsync(int asnId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .FirstOrDefaultAsync(r => r.AsnId == asnId && !r.IsDeleted);
        }

        public async Task<ReceiptDocumentation?> GetByInvoiceAsync(int warehouseId, string invoiceNumber)
        {
            return await _context.Set<ReceiptDocumentation>()
                .FirstOrDefaultAsync(r => r.WarehouseId == warehouseId &&
                                          r.InvoiceNumber == invoiceNumber &&
                                          !r.IsDeleted);
        }

        public async Task<ReceiptDocumentation?> GetByExternalReferenceAsync(int warehouseId, string externalReference)
        {
            return await _context.Set<ReceiptDocumentation>()
                .FirstOrDefaultAsync(r => r.WarehouseId == warehouseId &&
                                          r.ExternalReference == externalReference &&
                                          !r.IsDeleted);
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetByOperatorAsync(int warehouseId, int operatorId, DateTime? date = null)
        {
            var query = _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && r.OperatorId == operatorId && !r.IsDeleted);

            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(r => r.ReceiptDate >= startDate && r.ReceiptDate < endDate);
            }

            return await query.OrderByDescending(r => r.ReceiptDate).ToListAsync();
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetConfirmedBySupervisorAsync(int warehouseId, int supervisorId, DateTime? date = null)
        {
            var query = _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && r.SupervisorId == supervisorId && !r.IsDeleted);

            if (date.HasValue)
            {
                var startDate = date.Value.Date;
                var endDate = startDate.AddDays(1);
                query = query.Where(r => r.ConfirmedAt.HasValue &&
                                        r.ConfirmedAt.Value >= startDate && r.ConfirmedAt.Value < endDate);
            }

            return await query.OrderByDescending(r => r.ConfirmedAt).ToListAsync();
        }

        public async Task<int> CountInProgressAsync(int warehouseId)
        {
            var inProgressStatuses = new[] { 2, 6 };
            return await _context.Set<ReceiptDocumentation>()
                .CountAsync(r => r.WarehouseId == warehouseId &&
                                 inProgressStatuses.Contains((int)r.Status) &&
                                 !r.IsDeleted);
        }

        public async Task<int> CountPendingConfirmationAsync(int warehouseId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .CountAsync(r => r.WarehouseId == warehouseId &&
                                 ((int)r.Status == 1 || (int)r.Status == 2) &&
                                 !r.ConfirmedAt.HasValue &&
                                 !r.IsDeleted);
        }

        public async Task<int> CountWithDiscrepanciesAsync(int warehouseId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .CountAsync(r => r.WarehouseId == warehouseId && r.HasDiscrepancies && !r.IsDeleted);
        }

        public async Task<int> GetAverageReceiptTimeAsync(int warehouseId)
        {
            var receipts = await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId &&
                            r.TotalReceiptTimeMinutes.HasValue &&
                            !r.IsDeleted)
                .ToListAsync();

            if (receipts.Count == 0)
                return 0;

            return (int)receipts.Average(r => r.TotalReceiptTimeMinutes ?? 0);
        }

        public async Task<ReceiptDocumentation?> GetWithItemsAsync(Guid receiptId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == receiptId && !r.IsDeleted);
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetUnfinishedAsync(int warehouseId)
        {
            var unfinishedStatuses = new[] { 1, 2, 6 }; // Draft, InProgress, OnHold
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId &&
                            unfinishedStatuses.Contains((int)r.Status) &&
                            !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateStatusAsync(Guid receiptId, int newStatus)
        {
            var receipt = await GetByIdAsync(receiptId);
            if (receipt == null)
                return false;

            receipt.Status = (ReceiptStatus)newStatus;
            return true;
        }

        public async Task<IEnumerable<ReceiptDocumentation>> GetOnHoldAsync(int warehouseId)
        {
            return await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && (int)r.Status == 6 && !r.IsDeleted)
                .OrderByDescending(r => r.ReceiptDate)
                .ToListAsync();
        }

        public async Task<string> GetNextReceiptNumberAsync(int warehouseId)
        {
            var lastReceipt = await _context.Set<ReceiptDocumentation>()
                .Where(r => r.WarehouseId == warehouseId && !r.IsDeleted)
                .OrderByDescending(r => r.Id)
                .FirstOrDefaultAsync();

            if (lastReceipt == null)
            {
                return $"REC-{warehouseId:D3}-{DateTime.UtcNow:yyyyMMdd}-0001";
            }

            // Extrair número sequencial da última receipt e incrementar
            var lastNumber = int.Parse(lastReceipt.ReceiptNumber.Split('-').Last());
            return $"REC-{warehouseId:D3}-{DateTime.UtcNow:yyyyMMdd}-{lastNumber + 1:D4}";
        }
    }
}
