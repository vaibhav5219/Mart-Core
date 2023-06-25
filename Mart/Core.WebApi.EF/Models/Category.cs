using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public string ShopCode { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
