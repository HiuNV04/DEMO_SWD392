using System;
using System.Collections.Generic;

namespace DEMO_SWD392.Models;

public partial class BackupLog
{
    public int BackupId { get; set; }

    public DateTime? Timestamp { get; set; }

    public string? BackupStatus { get; set; }

    public string? BackupFilePath { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
