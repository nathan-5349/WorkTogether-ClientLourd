using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Technician
{
    public string Phone { get; set; } = null!;

    public short Level { get; set; }

    public int Id { get; set; }

    public virtual User IdNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
