using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

/// <summary>
/// Stock levels per product per location
/// </summary>
public class ProductStock : BaseEntity
{
    [Required]
    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal Quantity { get; set; } = 0;

    [MaxLength(100)]
    public string? LotNumber { get; set; }

    public DateTime? ExpiryDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey(nameof(ProductVariantId))]
    public virtual ProductVariant? ProductVariant { get; set; }

    [ForeignKey(nameof(LocationId))]
    public virtual Location Location { get; set; } = null!;
}

/// <summary>
/// History of all stock movements
/// </summary>
public class StockHistory : BaseEntity
{
    [Required]
    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    [MaxLength(50)]
    public string TransactionType { get; set; } = string.Empty; // Sale, Purchase, Transfer, Adjustment

    [Column(TypeName = "decimal(18,3)")]
    public decimal Quantity { get; set; } = 0;

    [Column(TypeName = "decimal(18,3)")]
    public decimal BalanceQuantity { get; set; } = 0;

    public int? ReferenceId { get; set; }

    [MaxLength(200)]
    public string? ReferenceType { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey(nameof(LocationId))]
    public virtual Location Location { get; set; } = null!;
}
