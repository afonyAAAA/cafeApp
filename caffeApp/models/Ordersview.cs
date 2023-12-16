using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Ordersview
{
    public int? OrderId { get; set; }

    public int? Quantityclients { get; set; }

    public DateOnly? Dateorder { get; set; }

    public string? Numberplace { get; set; }

    public int? UserId { get; set; }

    public bool? Isnoncash { get; set; }

    public decimal? Sum { get; set; }

    public string? Status { get; set; }

    public int? WorkshiftId { get; set; }
}
