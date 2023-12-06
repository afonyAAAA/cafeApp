using System;
using System.Collections.Generic;

namespace caffeApp.Desktop;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Secondname { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int DocumentId { get; set; }

    public int RoleId { get; set; }

    public bool? Isfired { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<Userworkshift> Userworkshifts { get; set; } = new List<Userworkshift>();
}
