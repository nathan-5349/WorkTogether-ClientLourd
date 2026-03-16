using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class User
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Roles { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Discr { get; set; } = null!;

    public virtual Accountant? Accountant { get; set; }

    public virtual Administrator? Administrator { get; set; }

    public virtual Company? Company { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Notice> Notices { get; set; } = new List<Notice>();

    public virtual Particular? Particular { get; set; }

    public virtual Support? Support { get; set; }

    public virtual Technician? Technician { get; set; }
}
