using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Intervention
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime BeginDate { get; set; }

    public DateTime FinishDate { get; set; }

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
