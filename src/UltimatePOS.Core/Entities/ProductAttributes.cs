using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UltimatePOS.Core.Entities;

public class Brand : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
   public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Category : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}

public class SubCategory : BaseEntity
{
    [Required]
    public int CategoryId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Unit : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ShortName { get; set; } = string.Empty;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
