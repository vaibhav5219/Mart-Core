using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public string Remarks { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? StreetNumber { get; set; }

    public string? CustomerUserName { get; set; }

    public string ShopCode { get; set; } = null!;

    public string PostalCode { get; set; } = null!;
}
