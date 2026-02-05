using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

public enum PaymentMethod
{
    Cash,
    Card,
    MobilePay,
    Credit,
    BankTransfer,
    Check
}

/// <summary>
/// Payment entity for both sales and purchases
/// </summary>
public class Payment : BaseEntity
{
    public int? SaleId { get; set; }

    public int? PurchaseOrderId { get; set; }

    public int? ContactId { get; set; }

    [Required]
    [MaxLength(50)]
    public string PaymentNumber { get; set; } = string.Empty;

    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } = 0;

    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;

    [MaxLength(200)]
    public string? ReferenceNumber { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int? PaymentAccountId { get; set; }

    // Navigation properties
    [ForeignKey("SaleId")]
    public virtual Sale? Sale { get; set; }

    [ForeignKey(nameof(PurchaseOrderId))]
    public virtual PurchaseOrder? PurchaseOrder { get; set; }

    [ForeignKey(nameof(ContactId))]
    public virtual Contact? Contact { get; set; }

    [ForeignKey(nameof(PaymentAccountId))]
    public virtual PaymentAccount? PaymentAccount { get; set; }
}

/// <summary>
/// Payment accounts (Cash, Bank accounts)
/// </summary>
public class PaymentAccount : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? AccountNumber { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; } = 0;

    public bool IsActive { get; set; } = true;
}
