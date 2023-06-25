using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class OrderStatusTbl
{
    public int OrderStatusId { get; set; }

    public string OrderStatus { get; set; } = null!;
}
