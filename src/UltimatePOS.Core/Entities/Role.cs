using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UltimatePOS.Core.Entities;

/// <summary>
/// Role entity for RBAC (Role-Based Access Control)
/// </summary>
public class Role : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// JSON string containing permissions array
    /// Example: ["products.view", "products.create", "sales.view", "sales.create"]
    /// </summary>
    public string Permissions { get; set; } = "[]";

    public bool IsSystemRole { get; set; } = false;

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
