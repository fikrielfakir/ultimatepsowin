using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UltimatePOS.Core.Entities;

/// <summary>
/// Represents a business/shop in the multi-business POS system
/// </summary>
public class Business : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? TaxNumber { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "USD";

    [MaxLength(100)]
    public string Timezone { get; set; } = "UTC";

    public int FinancialYearStartMonth { get; set; } = 1;

    public bool TaxInclusive { get; set; } = false;

    public decimal DefaultTaxRate { get; set; } = 0;

    [MaxLength(500)]
    public string? LogoPath { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
