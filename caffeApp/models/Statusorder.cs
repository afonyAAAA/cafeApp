using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Statusorder
{
    public int StatusorderId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
