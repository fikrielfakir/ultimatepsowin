namespace UltimatePOS.Core.Models;

/// <summary>
/// Filter criteria for product queries
/// </summary>
public class ProductFilter
{
    /// <summary>
    /// Search term for product name, SKU, or barcode
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Filter by category ID
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>
    /// Filter by brand ID
    /// </summary>
    public int? BrandId { get; set; }

    /// <summary>
    /// Filter by product type
    /// </summary>
    public Entities.ProductType? Type { get; set; }

    /// <summary>
    /// Filter by location ID (for stock filtering)
    /// </summary>
    public int? LocationId { get; set; }

    /// <summary>
    /// Show only low stock products
    /// </summary>
    public bool? LowStock { get; set; }

    /// <summary>
    /// Show only active products
    /// </summary>
    public bool? IsActive { get; set; } = true;

    /// <summary>
    /// Current page number (1-indexed)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Sort field (Name, SKU, CostPrice, SellingPrice, CreatedAt)
    /// </summary>
    public string SortBy { get; set; } = "Name";

    /// <summary>
    /// Sort in descending order
    /// </summary>
    public bool Descending { get; set; } = false;
}
