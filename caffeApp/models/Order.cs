using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Order
{
    public int OrderId { get; set; }

    public int Quantityclients { get; set; }

    public DateOnly Dateorder { get; set; }

    public int StatusorderId { get; set; }

    public int PlaceId { get; set; }

    public int? PaymentId { get; set; }

    public int UserId { get; set; }

    public int WorkshiftId { get; set; }

    public virtual Payment? Payment { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual Statusorder Statusorder { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Workshift Workshift { get; set; } = null!;
}
