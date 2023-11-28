using System;
using System.Collections.Generic;

namespace caffeApp;

public partial class Document
{
    public int DocumentId { get; set; }

    public string Contractlink { get; set; } = null!;

    public string Photolink { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
