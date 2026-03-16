using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Particular
{
    public string Gender { get; set; } = null!;

    public int Id { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
