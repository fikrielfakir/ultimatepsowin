using System.Collections.Generic;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;

namespace UltimatePOS.Core.Interfaces;

public interface IStockService
{
    // Stock Viewing
    Task<decimal> GetCurrentStockAsync(int productId, int? locationId = null);
    Task<IEnumerable<ProductStock>> GetStockLevelsAsync(int productId);
    Task<IEnumerable<ProductStock>> GetStockByLocationAsync(int locationId);
    Task<IEnumerable<ProductStock>> GetLowStockProductsAsync(int locationId);

    // Stock Operations
    Task AdjustStockAsync(int productId, int locationId, decimal quantity, string type, string reason, string? reference = null);
    Task TransferStockAsync(int productId, int fromLocationId, int toLocationId, decimal quantity, string? reference = null);
    
    // History
    Task<IEnumerable<StockHistory>> GetStockHistoryAsync(int productId, System.DateTime? fromDate, System.DateTime? toDate);

    // Stock Take
    Task<StockTake> CreateStockTakeAsync(int locationId, string? notes = null);
    Task UpdateStockTakeDetailAsync(int stockTakeId, int productId, decimal countedQty, string? notes = null);
    Task<StockTake> CompleteStockTakeAsync(int stockTakeId, bool autoAdjust = false);
    Task<IEnumerable<StockTake>> GetStockTakesAsync(int? locationId = null, System.DateTime? fromDate = null, System.DateTime? toDate = null);
    Task<StockTake?> GetStockTakeByIdAsync(int stockTakeId);
}
