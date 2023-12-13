using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Statuspayment
{
    public int StatuspaymentId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
