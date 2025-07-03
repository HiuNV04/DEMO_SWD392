using System;
using System.Collections.Generic;

namespace DEMO_SWD392.Models;

public partial class DiscountCode
{
    public string DiscountCode1 { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Percentage { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
