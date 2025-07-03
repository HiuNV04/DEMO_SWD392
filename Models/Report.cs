using System;
using System.Collections.Generic;

namespace DEMO_SWD392.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public string? ReportType { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public string? ReportFilePath { get; set; }

    public int? RelatedWorkshiftId { get; set; }

    public virtual User? CreatedByNavigation { get; set; }
}
