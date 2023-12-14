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

    public int UserworkshiftId { get; set; }

    public virtual ICollection<Foodorder> Foodorders { get; set; } = new List<Foodorder>();

    public virtual Payment? Payment { get; set; }

    public virtual Place Place { get; set; } = null!;

    public virtual Statusorder Statusorder { get; set; } = null!;

    public virtual UserWorkShift Userworkshift { get; set; } = null!;

}
