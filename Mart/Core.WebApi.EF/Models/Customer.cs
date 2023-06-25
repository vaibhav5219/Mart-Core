using System;
using System.Collections.Generic;

namespace Core.EF.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Mobile { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public int? Address { get; set; }

    public string AspNetUserId { get; set; } = null!;
}
