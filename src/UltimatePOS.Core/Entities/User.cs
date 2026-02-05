using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UltimatePOS.Core.Entities;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User : BaseEntity
{
    [Required]
    public int BusinessId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    public int? RoleId { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsCommissionAgent { get; set; } = false;

    public decimal CommissionRate { get; set; } = 0;

    [MaxLength(500)]
    public string? AssignedLocationIds { get; set; }

    // Navigation properties
    [ForeignKey(nameof(BusinessId))]
    public virtual Business Business { get; set; } = null!;

    [ForeignKey(nameof(RoleId))]
    public virtual Role? Role { get; set; }
}
