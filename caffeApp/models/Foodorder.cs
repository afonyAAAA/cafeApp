using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Foodorder
{
    public int FoodorderId { get; set; }

    public int FoodId { get; set; }

    public int OrderId { get; set; }

    public virtual Food Food { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
