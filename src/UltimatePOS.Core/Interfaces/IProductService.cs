using System.Collections.Generic;
using System.Threading.Tasks;
using UltimatePOS.Core.Entities;
using UltimatePOS.Core.Models;

namespace UltimatePOS.Core.Interfaces;

/// <summary>
/// Service for managing products
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Get products with filtering and pagination
    /// </summary>
    Task<PagedResult<Product>> GetProductsAsync(ProductFilter filter);

    /// <summary>
    /// Get product by ID
    /// </summary>
    Task<Product?> GetProductByIdAsync(int id);

    /// <summary>
    /// Create a new product
    /// </summary>
    Task<Product> CreateProductAsync(Product product);

    /// <summary>
    /// Update an existing product
    /// </summary>
    Task UpdateProductAsync(Product product);

    /// <summary>
    /// Delete a product
    /// </summary>
    Task DeleteProductAsync(int id);

    /// <summary>
    /// Check if a product exists with the given SKU
    /// </summary>
    Task<bool> ProductExistsBySkuAsync(string sku, int? excludeId = null);

    /// <summary>
    /// Get all brands
    /// </summary>
    Task<IEnumerable<Brand>> GetBrandsAsync();

    /// <summary>
    /// Get all categories
    /// </summary>
    Task<IEnumerable<Category>> GetCategoriesAsync();

    /// <summary>
    /// Get all units
    /// </summary>
    Task<IEnumerable<Unit>> GetUnitsAsync();

    /// <summary>
    /// Get all products for a specific business
    /// </summary>
    Task<IEnumerable<Product>> GetProductsByBusinessIdAsync(int businessId);
}
