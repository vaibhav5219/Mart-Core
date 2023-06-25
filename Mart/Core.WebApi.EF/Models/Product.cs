using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? ImagePath { get; set; }

    public double? UnitPrice { get; set; }

    public int? CategoryId { get; set; }

    public string? ShopCode { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ShopDetail? ShopCodeNavigation { get; set; }
}
