using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public int OrderTotal { get; set; }

    public int CustomerId { get; set; }

    public string ShopCode { get; set; } = null!;

    public int? ProductId { get; set; }

    public int OrderQuantity { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int? PaymentId { get; set; }

    public DateTime? ShipDate { get; set; }

    public DateTime? RequiredDate { get; set; }

    public int? ShipperId { get; set; }

    public string? Freight { get; set; }

    public int? SalesTax { get; set; }

    public int? TransactStatus { get; set; }

    public bool? IsCancled { get; set; }

    public string? Paid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public bool? IsOrderConfimed { get; set; }

    public bool? IsShipped { get; set; }

    public bool? IsOutForDelivery { get; set; }

    public bool? IsDelivered { get; set; }

    public DateTime? OutForDeliveryDate { get; set; }

    public DateTime? DeliveredDate { get; set; }

    public int? RefundId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product? Product { get; set; }

    public virtual ShopDetail ShopCodeNavigation { get; set; } = null!;
}
