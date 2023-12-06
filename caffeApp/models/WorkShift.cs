using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Workshift
{
    public int WorkshiftId { get; set; }

    public TimeOnly Timestart { get; set; }

    public TimeOnly Timeend { get; set; }

    public DateOnly Date { get; set; }

    public virtual ICollection<UserWorkShift> Userworkshifts { get; set; } = new List<UserWorkShift>();
}
