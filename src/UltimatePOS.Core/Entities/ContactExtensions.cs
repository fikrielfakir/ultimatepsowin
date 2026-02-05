using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UltimatePOS.Core.Entities;

public class ContactGroup : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}

public class PaymentTerm : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public int DaysUntilDue { get; set; } = 0;

    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}
