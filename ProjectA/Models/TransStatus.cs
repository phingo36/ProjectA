using System;
using System.Collections.Generic;

namespace ProjectA.Models;

public partial class TransStatus
{
    public int TransStatusId { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
