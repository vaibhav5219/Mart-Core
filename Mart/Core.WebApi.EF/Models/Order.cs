using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int OrderTotal { get; set; }

    public int OrderStatus { get; set; }

    public string CustomerId { get; set; } = null!;

    public DateTime? DeliveredDate { get; set; }

    public string ShopCode { get; set; } = null!;

    public int ProductId { get; set; }

    public int OrderQuantity { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ShopDetail ShopCodeNavigation { get; set; } = null!;
}
