using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

public enum ContactType
{
    Customer,
    Supplier,
    Both
}

/// <summary>
/// Contact entity for customers and suppliers
/// </summary>
public class Contact : BaseEntity
{
    [Required]
    public int BusinessId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public ContactType Type { get; set; } = ContactType.Customer;

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? TaxNumber { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal CreditLimit { get; set; } = 0;

    public int? PaymentTermId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal OpeningBalance { get; set; } = 0;

    public int? ContactGroupId { get; set; }

    public int LoyaltyPoints { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(BusinessId))]
    public virtual Business Business { get; set; } = null!;

    [ForeignKey(nameof(PaymentTermId))]
    public virtual PaymentTerm? PaymentTerm { get; set; }

    [ForeignKey(nameof(ContactGroupId))]
    public virtual ContactGroup? ContactGroup { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
