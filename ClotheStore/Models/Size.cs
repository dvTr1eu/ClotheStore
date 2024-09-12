using System;
using System.Collections.Generic;

namespace ClotheStore.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public string? SizeName { get; set; }

    public virtual ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
}
