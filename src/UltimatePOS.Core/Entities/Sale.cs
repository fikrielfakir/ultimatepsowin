using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

public enum SaleStatus
{
    Draft,
    Confirmed,
    Delivered,
    Cancelled
}

public enum PaymentStatus
{
    Unpaid,
    PartiallyPaid,
    Paid,
    Overdue
}

/// <summary>
/// Sale/Invoice entity
/// </summary>
public class Sale : BaseEntity
{
    [Required]
    public int BusinessId { get; set; }

    [Required]
    public int LocationId { get; set; }

    public int? ContactId { get; set; }

    [Required]
    [MaxLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;

    public SaleStatus Status { get; set; } = SaleStatus.Confirmed;

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

    public int? CashierId { get; set; }

    // Navigation properties
    [ForeignKey(nameof(BusinessId))]
    public virtual Business Business { get; set; } = null!;

    [ForeignKey(nameof(LocationId))]
    public virtual Location Location { get; set; } = null!;

    [ForeignKey(nameof(ContactId))]
    public virtual Contact? Contact { get; set; }

    [ForeignKey(nameof(CashierId))]
    public virtual User? Cashier { get; set; }

    public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

/// <summary>
/// Sale item (line item)
/// </summary>
public class SaleItem : BaseEntity
{
    [Required]
    public int SaleId { get; set; }

    [Required]
    public int ProductId { get; set; }

    public int? ProductVariantId { get; set; }

    [Column(TypeName = "decimal(18,3)")]
    public decimal Quantity { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Subtotal { get; set; } = 0;

    // Navigation properties
    [ForeignKey(nameof(SaleId))]
    public virtual Sale Sale { get; set; } = null!;

    [ForeignKey(nameof(ProductId))]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey(nameof(ProductVariantId))]
    public virtual ProductVariant? ProductVariant { get; set; }
}
