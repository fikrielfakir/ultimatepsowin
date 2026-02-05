using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

/// <summary>
/// Product variant for variable products (e.g., size/color combinations)
/// </summary>
public class ProductVariant : BaseEntity
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(200)]
    public string VariantName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? SKU { get; set; }

    [MaxLength(200)]
    public string? Barcode { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal AdditionalCost { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal AdditionalPrice { get; set; } = 0;

    // Navigation properties
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;
}
