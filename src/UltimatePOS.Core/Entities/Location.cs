using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

/// <summary>
/// Represents a physical location/warehouse for a business
/// </summary>
public class Location : BaseEntity
{
    [Required]
    public int BusinessId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(500)]
    public string? InvoiceLayoutSettings { get; set; }

    [MaxLength(200)]
    public string? ReceiptPrinterName { get; set; }

    [MaxLength(200)]
    public string? LabelPrinterName { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    [ForeignKey(nameof(BusinessId))]
    public virtual Business Business { get; set; } = null!;
    
    public virtual ICollection<ProductStock> ProductStocks { get; set; } = new List<ProductStock>();
    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
