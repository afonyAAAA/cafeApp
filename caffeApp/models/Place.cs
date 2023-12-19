using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Place
{
    public int PlaceId { get; set; }

    public string Number { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Userworkshift> Userworkshifts { get; set; } = new List<Userworkshift>();
}
