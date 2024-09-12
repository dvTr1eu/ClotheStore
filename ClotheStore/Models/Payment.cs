using System;
using System.Collections.Generic;

namespace ClotheStore.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public string? Payment1 { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
