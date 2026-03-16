using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Administrator
{
    public int Id { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
