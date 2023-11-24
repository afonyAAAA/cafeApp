using System;
using System.Collections.Generic;

namespace caffeApp.models;

public partial class User
{
    public int UserId { get; set; }

    public string Firstname { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int DocumentId { get; set; }

    public int RoleId { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
