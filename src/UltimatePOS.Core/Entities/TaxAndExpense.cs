using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

/// <summary>
/// Tax rate entity
/// </summary>
public class TaxRate : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column(TypeName = "decimal(5,2)")]
    public decimal Rate { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

/// <summary>
/// Expense category
/// </summary>
public class ExpenseCategory : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}

/// <summary>
/// Expense entity
/// </summary>
public class Expense : BaseEntity
{
    [Required]
    public int BusinessId { get; set; }

    public int? LocationId { get; set; }

    [Required]
    public int ExpenseCategoryId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; } = 0;

    public System.DateTime ExpenseDate { get; set; } = System.DateTime.UtcNow;

    [MaxLength(200)]
    public string? PaidTo { get; set; }

    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    [MaxLength(500)]
    public string? ReceiptPath { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public bool IsTaxDeductible { get; set; } = false;

    // Navigation properties
    [ForeignKey(nameof(ExpenseCategoryId))]
    public virtual ExpenseCategory ExpenseCategory { get; set; } = null!;

    [ForeignKey(nameof(LocationId))]
    public virtual Location? Location { get; set; }
}
