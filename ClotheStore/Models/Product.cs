using System;
using System.Collections.Generic;

namespace ClotheStore.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? ProductName { get; set; }

    public string? Description { get; set; }

    public int? Price { get; set; }

    public int? Discount { get; set; }

    public DateTime? CreateDate { get; set; }

    public int CatId { get; set; }

    public bool BestSeller { get; set; }

    public bool HomeFlag { get; set; }

    public bool Active { get; set; }

    public string? Title { get; set; }

    public string? Alias { get; set; }

    public virtual Category Cat { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}
