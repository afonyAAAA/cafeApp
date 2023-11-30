using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace caffeApp.Desktop;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public int DocumentId { get; set; }

    public int RoleId { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;


    public string getFullName() => FirstName + " " + SecondName + " " + Surname;
    
}
