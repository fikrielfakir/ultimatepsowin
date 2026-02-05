using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

public enum PurchaseStatus
{
    Draft,
    Ordered,
    Received,
    Cancelled
}

/// <summary>
/// Purchase order entity
/// </summary>
public class PurchaseOrder : BaseEntity
{
    [Required]
    public int BusinessId { get; set; }

    [Required]
    public int LocationId { get; set; }

    [Required]
    public int SupplierId { get; set; }

    [Required]
    [MaxLength(50)]
    public string OrderNumber { get; set; } = string.Empty;

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public DateTime? ExpectedDeliveryDate { get; set; }

    public PurchaseStatus Status { get; set; } = PurchaseStatus.Draft;

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal PaidAmount { get; set; } = 0;

    [MaxLength(1000)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(BusinessId))]
    public virtual Business Business { get; set; } = null!;

    [ForeignKey(nameof(LocationId))]
    public virtual Location Location { get; set; } = null!;

    [ForeignKey(nameof(SupplierId))]
    public virtual Contact Supplier { get; set; } = null!;

    public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

/// <summary>
/// Purchase order item
/// </summary>
public class PurchaseOrderItem : BaseEntity
{
    [Required]
    public int PurchaseOrderId { get; set; }

    [Required]
    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal QuantityOrdered { get; set; } = 0;

    [Column(TypeName = "decimal(18,3)")]
    public decimal QuantityReceived { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitCost { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; } = 0;

    [MaxLength(100)]
    public string? LotNumber { get; set; }

    public DateTime? ExpiryDate { get; set; }

    // Navigation properties
    [ForeignKey(nameof(PurchaseOrderId))]
    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey(nameof(ProductVariantId))]
    public virtual ProductVariant? ProductVariant { get; set; }
}
