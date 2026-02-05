using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

public enum ProductType
{
    Single,
    Variable,
    LotBased
}

/// <summary>
/// Product entity
/// </summary>
public class Product : BaseEntity
{
    [Required]
    public int BusinessId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? SKU { get; set; }

    [MaxLength(200)]
    public string? Barcode { get; set; }

    public ProductType Type { get; set; } = ProductType.Single;

    public int? BrandId { get; set; }

    public int? CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public int? UnitId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CostPrice { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SellingPrice { get; set; } = 0;

    [Column(TypeName = "decimal(5,2)")]
    public decimal ProfitMargin { get; set; } = 0;

    public int? TaxRateId { get; set; }

    public decimal AlertQuantity { get; set; } = 0;

    [MaxLength(50)]
    public string? RackPosition { get; set; }

    public int? WarrantyPeriodDays { get; set; }

    public bool HasExpiryDate { get; set; } = false;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(BrandId))]
    public virtual Brand? Brand { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual Category? Category { get; set; }

    [ForeignKey(nameof(SubCategoryId))]
    public virtual SubCategory? SubCategory { get; set; }

    [ForeignKey(nameof(UnitId))]
    public virtual Unit? Unit { get; set; }

    [ForeignKey(nameof(TaxRateId))]
    public virtual TaxRate? TaxRate { get; set; }

    public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public virtual ICollection<ProductStock> Stocks { get; set; } = new List<ProductStock>();
    public virtual ICollection<SellingPriceGroup> SellingPriceGroups { get; set; } = new List<SellingPriceGroup>();
}
