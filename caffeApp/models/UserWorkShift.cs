﻿using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Userworkshift
{
    public int UserworkshiftId { get; set; }

    public int WorkshiftId { get; set; }

    public int UserId { get; set; }

    public int? PlaceId { get; set; }

    public virtual Place? Place { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Workshift Workshift { get; set; } = null!;
}
