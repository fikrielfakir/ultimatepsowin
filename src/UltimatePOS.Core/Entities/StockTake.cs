using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace UltimatePOS.Core.Entities;

public enum StockTakeStatus
{
    InProgress,
    Completed,
    Cancelled
}

/// <summary>
/// Stock take header - represents a physical inventory count session
/// </summary>
public class StockTake : BaseEntity
{
    [Required]
    public int LocationId { get; set; }

    [Required]
    public DateTime StockTakeDate { get; set; } = DateTime.UtcNow;

    [Required]
    public StockTakeStatus Status { get; set; } = StockTakeStatus.InProgress;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public bool AutoAdjusted { get; set; } = false;

    public DateTime? CompletedDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(LocationId))]
    public virtual Location Location { get; set; } = null!;

    public virtual ICollection<StockTakeDetail> Details { get; set; } = new List<StockTakeDetail>();
}

/// <summary>
/// Stock take detail - individual product counts
/// </summary>
public class StockTakeDetail : BaseEntity
{
    [Required]
    public int StockTakeId { get; set; }

    [Required]
    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal ExpectedQuantity { get; set; } = 0;

    [Column(TypeName = "decimal(18,3)")]
    public decimal CountedQuantity { get; set; } = 0;

    [Column(TypeName = "decimal(18,3)")]
    public decimal Variance { get; set; } = 0;

    [MaxLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(StockTakeId))]
    public virtual StockTake StockTake { get; set; } = null!;

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey(nameof(ProductVariantId))]
    public virtual ProductVariant? ProductVariant { get; set; }
}
