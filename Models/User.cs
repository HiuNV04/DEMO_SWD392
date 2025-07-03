using System;
using System.Collections.Generic;

namespace DEMO_SWD392.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? AccountFullName { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<BackupLog> BackupLogs { get; set; } = new List<BackupLog>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual Role? Role { get; set; }
}
