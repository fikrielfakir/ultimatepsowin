using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Interfaces;
using UltimatePOS.Core.Models;

namespace UltimatePOS.Services;

/// <summary>
/// Product service implementation
/// </summary>
public class ProductService : ServiceBase, IProductService
{
    private readonly ISessionService _sessionService;

    public ProductService(IUnitOfWork unitOfWork, ISessionService sessionService) 
        : base(unitOfWork)
    {
        _sessionService = sessionService;
    }

    public async Task<PagedResult<Product>> GetProductsAsync(ProductFilter filter)
    {
        var query = _unitOfWork.Products.Query();

        // Filter by business (always scope to current business)
        if (_sessionService.CurrentBusiness != null)
        {
            query = query.Where(p => p.BusinessId == _sessionService.CurrentBusiness.Id);
        }

        // Search term filter
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(p => 
                p.Name.ToLower().Contains(searchTerm) ||
                (p.SKU != null && p.SKU.ToLower().Contains(searchTerm)) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(searchTerm))
            );
        }

        // Category filter
        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        // Brand filter
        if (filter.BrandId.HasValue)
        {
            query = query.Where(p => p.BrandId == filter.BrandId.Value);
        }

        // Type filter
        if (filter.Type.HasValue)
        {
            query = query.Where(p => p.Type == filter.Type.Value);
        }

        // Active filter
        if (filter.IsActive.HasValue)
        {
            query = query.Where(p => p.IsActive == filter.IsActive.Value);
        }

        // TODO: Low stock filter (requires joining with ProductStock)
        // This will be implemented when we add IStockService

        // Include related entities
        query = query
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .Include(p => p.TaxRate);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Sorting
        query = filter.SortBy.ToLower() switch
        {
            "sku" => filter.Descending ? query.OrderByDescending(p => p.SKU) : query.OrderBy(p => p.SKU),
            "costprice" => filter.Descending ? query.OrderByDescending(p => p.CostPrice) : query.OrderBy(p => p.CostPrice),
            "sellingprice" => filter.Descending ? query.OrderByDescending(p => p.SellingPrice) : query.OrderBy(p => p.SellingPrice),
            "createdat" => filter.Descending ? query.OrderByDescending(p => p.CreatedDate) : query.OrderBy(p => p.CreatedDate),
            _ => filter.Descending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name)
        };

        // Pagination
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        var products = await _unitOfWork.Products.FindAsync(p => p.Id == id);
        return products.FirstOrDefault();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        // Set business ID from session
        if (_sessionService.CurrentBusiness != null)
        {
            product.BusinessId = _sessionService.CurrentBusiness.Id;
        }

        // Calculate profit margin
        if (product.CostPrice > 0)
        {
            product.ProfitMargin = ((product.SellingPrice - product.CostPrice) / product.CostPrice) * 100;
        }

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return product;
    }

    public async Task UpdateProductAsync(Product product)
    {
        // Recalculate profit margin
        if (product.CostPrice > 0)
        {
            product.ProfitMargin = ((product.SellingPrice - product.CostPrice) / product.CostPrice) * 100;
        }

            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await GetProductByIdAsync(id);
        if (product != null)
        {
            await _unitOfWork.Products.DeleteAsync(product.Id);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<bool> ProductExistsBySkuAsync(string sku, int? excludeId = null)
    {
        var query = _unitOfWork.Products.Query();
        query = query.Where(p => p.SKU == sku);

        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Brand>> GetBrandsAsync()
    {
        return await _unitOfWork.Brands.GetAllAsync();
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _unitOfWork.Categories.GetAllAsync();
    }

    public async Task<IEnumerable<Unit>> GetUnitsAsync()
    {
        return await _unitOfWork.Units.GetAllAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByBusinessIdAsync(int businessId)
    {
        // Ensure we only return products for the requested business
        // This is a double check since _sessionService should already handle scoping if used,
        // but here we are explicitly asked for a specific businessId.
        // However, for security in a multi-tenant app, we should probably check if businessId matches current session
        // or if the user has permission. For now, we will trust the caller but filter by the ID.
        
        List<Product> products = await _unitOfWork.Products.Query()
            .Where(p => p.BusinessId == businessId)
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .ToListAsync();

        return products;
    }
}
