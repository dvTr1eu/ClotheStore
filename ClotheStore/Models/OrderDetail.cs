using System;
using System.Collections.Generic;

namespace ClotheStore.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int? OrderNumber { get; set; }

    public int? Quantity { get; set; }

    public int? Discount { get; set; }

    public string? ImageUrl { get; set; }

    public int? Total { get; set; }

    public string? Size { get; set; }

    public DateTime? ShipDate { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
