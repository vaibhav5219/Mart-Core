using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class CartItem
{
    public string CartId { get; set; } = null!;

    public string? ItemId { get; set; }

    public int Quantity { get; set; }

    public DateTime DateCreated { get; set; }

    public int ProductId { get; set; }

    public string ShopCode { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
