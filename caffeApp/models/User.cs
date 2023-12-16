using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace caffeApp.Desktop;

public partial class User
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public bool IsFired { get; set; }

    public int? DocumentId { get; set; }

    public int RoleId { get; set; }

    [JsonIgnore]
    public virtual Document? Document { get; set; }

    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<Userworkshift> Userworkshifts { get; set; } = new List<Userworkshift>();

    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public string getFullName() => FirstName + " " + SecondName + " " + Surname;
}
