using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Workshiftview
{
    public DateOnly? Date { get; set; }

    public string? Time { get; set; }

    public string? Fullname { get; set; }

    public string? Rolename { get; set; }

    public int? UserId { get; set; }

    public int? WorkshiftId { get; set; }
}
