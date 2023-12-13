using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Food
{
    public int FoodId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<Foodorder> Foodorders { get; set; } = new List<Foodorder>();
}
