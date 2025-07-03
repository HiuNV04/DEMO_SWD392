using System;
using System.Collections.Generic;

namespace DEMO_SWD392.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public DateOnly? InvoiceDate { get; set; }

    public int? UserId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? DiscountCode { get; set; }

    public virtual DiscountCode? DiscountCodeNavigation { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual User? User { get; set; }
}
