using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

/// <summary>
/// Selling price groups (Wholesale, Retail, VIP, etc.)
/// </summary>
public class SellingPriceGroup : BaseEntity
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(100)]
    public string GroupName { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; } = 0;

    // Navigation properties
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;
}
