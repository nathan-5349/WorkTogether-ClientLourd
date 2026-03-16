using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Company
{
    public string Siret { get; set; } = null!;

    public string CompanyName { get; set; } = null!;

    public int Id { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
