using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class ShopDetail
{
    public string ShopId { get; set; } = null!;

    public string ShopCode { get; set; } = null!;

    public string ShopName { get; set; } = null!;

    public string ShopKeeperName { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public int Address { get; set; }

    public string ShopDomainName { get; set; } = null!;

    public string PinCode { get; set; } = null!;

    public string AspNetUsersId { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
