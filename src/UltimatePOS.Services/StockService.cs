using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;

namespace UltimatePOS.Services;

public class StockService : IStockService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionService _sessionService;

    public StockService(IUnitOfWork unitOfWork, ISessionService sessionService)
    {
        _unitOfWork = unitOfWork;
        _sessionService = sessionService;
    }

    public async Task<decimal> GetCurrentStockAsync(int productId, int? locationId = null)
    {
        var query = _unitOfWork.ProductStocks.Query()
            .Where(s => s.ProductId == productId);

        if (locationId.HasValue)
        {
            query = query.Where(s => s.LocationId == locationId.Value);
        }

        return await query.SumAsync(s => s.Quantity);
    }

    public async Task<IEnumerable<ProductStock>> GetStockLevelsAsync(int productId)
    {
        return await _unitOfWork.ProductStocks.Query()
            .Include(s => s.Location)
            .Where(s => s.ProductId == productId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductStock>> GetStockByLocationAsync(int locationId)
    {
        return await _unitOfWork.ProductStocks.Query()
            .Include(s => s.Product)
            .Where(s => s.LocationId == locationId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductStock>> GetLowStockProductsAsync(int locationId)
    {
        // Get stocks for location where quantity <= alert quantity
        // Note: AlertQuantity is on Product, Stock is on ProductStock
        // We join or include to filter
        
        var stocks = await _unitOfWork.ProductStocks.Query()
            .Include(s => s.Product)
            .Where(s => s.LocationId == locationId)
            .Where(s => s.Quantity <= s.Product.AlertQuantity && s.Product.AlertQuantity > 0)
            .ToListAsync();

        return stocks;
    }

    public async Task AdjustStockAsync(int productId, int locationId, decimal quantity, string type, string reason, string? reference = null)
    {
        // 1. Get or create stock record
        var stock = await _unitOfWork.ProductStocks.Query()
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.LocationId == locationId);

        if (stock == null)
        {
            stock = new ProductStock
            {
                ProductId = productId,
                LocationId = locationId,
                Quantity = 0
            };
            await _unitOfWork.ProductStocks.AddAsync(stock);
        }

        // 2. Update quantity
        decimal oldQty = stock.Quantity;
        stock.Quantity += quantity; // Quantity can be negative for removal

        if (stock.Quantity < 0)
        {
            throw new InvalidOperationException($"Insufficient stock. Current: {oldQty}, Adjustment: {quantity}");
        }

        await _unitOfWork.ProductStocks.UpdateAsync(stock);

        // 3. Log history
        var history = new StockHistory
        {
            ProductId = productId,
            LocationId = locationId,
            TransactionType = type ?? "Adjustment",
            Quantity = quantity,
            BalanceQuantity = stock.Quantity,
            ReferenceType = reference != null ? "Manual" : null,
            Notes = reason,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = _sessionService.CurrentUser?.Id.ToString()
        };
        await _unitOfWork.StockHistories.AddAsync(history);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task TransferStockAsync(int productId, int fromLocationId, int toLocationId, decimal quantity, string? reference = null)
    {
        if (quantity <= 0) throw new ArgumentException("Transfer quantity must be positive");
        
        if (fromLocationId == toLocationId) throw new ArgumentException("Source and destination locations must be different");

        // Use transaction via retry strategy or just simple implementation for now
        // Remove from source (using standard adjustment logic but custom type)
        await AdjustStockAsync(productId, fromLocationId, -quantity, "Transfer_Out", $"Transfer to location {toLocationId}", reference);

        // Add to destination
        await AdjustStockAsync(productId, toLocationId, quantity, "Transfer_In", $"Transfer from location {fromLocationId}", reference);
    }

    public async Task<IEnumerable<StockHistory>> GetStockHistoryAsync(int productId, DateTime? fromDate, DateTime? toDate)
    {
        var query = _unitOfWork.StockHistories.Query()
            .Include(h => h.Location)
            .Where(h => h.ProductId == productId);

        if (fromDate.HasValue)
            query = query.Where(h => h.CreatedDate >= fromDate.Value);
            
        if (toDate.HasValue)
            query = query.Where(h => h.CreatedDate <= toDate.Value);

        return await query.OrderByDescending(h => h.CreatedDate).ToListAsync();
    }

    public async Task<StockTake> CreateStockTakeAsync(int locationId, string? notes = null)
    {
        // Create stock take header
        var stockTake = new StockTake
        {
            LocationId = locationId,
            StockTakeDate = DateTime.UtcNow,
            Status = StockTakeStatus.InProgress,
            Notes = notes,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = _sessionService.CurrentUser?.Id.ToString()
        };

        await _unitOfWork.StockTakes.AddAsync(stockTake);
        await _unitOfWork.SaveChangesAsync();

        // Load all products with stock at this location
        var stocks = await _unitOfWork.ProductStocks.Query()
            .Include(s => s.Product)
            .Where(s => s.LocationId == locationId)
            .ToListAsync();

        // Create detail lines with expected quantities
        foreach (var stock in stocks)
        {
            var detail = new StockTakeDetail
            {
                StockTakeId = stockTake.Id,
                ProductId = stock.ProductId,
                ProductVariantId = stock.ProductVariantId,
                ExpectedQuantity = stock.Quantity,
                CountedQuantity = 0,
                Variance = 0,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = _sessionService.CurrentUser?.Id.ToString()
            };
            await _unitOfWork.StockTakeDetails.AddAsync(detail);
        }

        await _unitOfWork.SaveChangesAsync();
        return stockTake;
    }

    public async Task UpdateStockTakeDetailAsync(int stockTakeId, int productId, decimal countedQty, string? notes = null)
    {
        var detail = await _unitOfWork.StockTakeDetails.Query()
            .FirstOrDefaultAsync(d => d.StockTakeId == stockTakeId && d.ProductId == productId);

        if (detail == null)
        {
            throw new InvalidOperationException($"Stock take detail not found for product {productId}");
        }

        detail.CountedQuantity = countedQty;
        detail.Variance = countedQty - detail.ExpectedQuantity;
        detail.Notes = notes;
        detail.ModifiedDate = DateTime.UtcNow;
        detail.ModifiedBy = _sessionService.CurrentUser?.Id.ToString();

        await _unitOfWork.StockTakeDetails.UpdateAsync(detail);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<StockTake> CompleteStockTakeAsync(int stockTakeId, bool autoAdjust = false)
    {
        var stockTake = await _unitOfWork.StockTakes.Query()
            .Include(st => st.Details)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(st => st.Id == stockTakeId);

        if (stockTake == null)
        {
            throw new InvalidOperationException($"Stock take {stockTakeId} not found");
        }

        if (stockTake.Status != StockTakeStatus.InProgress)
        {
            throw new InvalidOperationException($"Stock take is not in progress");
        }

        // If auto-adjust, create adjustments for all variances
        if (autoAdjust)
        {
            foreach (var detail in stockTake.Details.Where(d => d.Variance != 0))
            {
                await AdjustStockAsync(
                    detail.ProductId,
                    stockTake.LocationId,
                    detail.Variance,
                    "StockTake",
                    $"Stock take adjustment - ST#{stockTakeId}",
                    $"StockTake-{stockTakeId}"
                );
            }
        }

        // Mark as completed
        stockTake.Status = StockTakeStatus.Completed;
        stockTake.CompletedDate = DateTime.UtcNow;
        stockTake.AutoAdjusted = autoAdjust;
        stockTake.ModifiedDate = DateTime.UtcNow;
        stockTake.ModifiedBy = _sessionService.CurrentUser?.Id.ToString();

        await _unitOfWork.StockTakes.UpdateAsync(stockTake);
        await _unitOfWork.SaveChangesAsync();

        return stockTake;
    }

    public async Task<IEnumerable<StockTake>> GetStockTakesAsync(int? locationId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _unitOfWork.StockTakes.Query()
            .Include(st => st.Location);

        if (locationId.HasValue)
            query = query.Where(st => st.LocationId == locationId.Value);

        if (fromDate.HasValue)
            query = query.Where(st => st.StockTakeDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(st => st.StockTakeDate <= toDate.Value);

        return await query.OrderByDescending(st => st.StockTakeDate).ToListAsync();
    }

    public async Task<StockTake?> GetStockTakeByIdAsync(int stockTakeId)
    {
        return await _unitOfWork.StockTakes.Query()
            .Include(st => st.Location)
            .Include(st => st.Details)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(st => st.Id == stockTakeId);
    }
}
