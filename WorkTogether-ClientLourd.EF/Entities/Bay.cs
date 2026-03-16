using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Bay
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CapacityUnit { get; set; }

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
