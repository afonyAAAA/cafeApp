using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Workshift
{
    public int WorkshiftId { get; set; }

    public TimeOnly Timestart { get; set; }

    public TimeOnly Timeend { get; set; }

    public DateOnly Date { get; set; }

    public virtual ICollection<Userworkshift> Userworkshifts { get; set; } = new List<Userworkshift>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
