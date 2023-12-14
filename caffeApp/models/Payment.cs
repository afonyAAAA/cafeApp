using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int StatuspaymentId { get; set; }

    public decimal Sum { get; set; }

    public bool Isnoncash { get; set; }

    public DateTimeOffset Datepayment { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Statuspayment Statuspayment { get; set; } = null!;
}
