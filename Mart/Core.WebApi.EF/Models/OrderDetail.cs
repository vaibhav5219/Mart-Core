using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class OrderDetail
{
    public int OrderDetailsId { get; set; }

    public string OrderId { get; set; } = null!;

    public int ProductId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int? Price { get; set; }

    public int Quantity { get; set; }

    public string? Discount { get; set; }

    public int? TotalPrice { get; set; }

    public string CustomerId { get; set; } = null!;
}
