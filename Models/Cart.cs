using System;
using System.Collections.Generic;

namespace DEMO_SWD392.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual User? User { get; set; }
}
