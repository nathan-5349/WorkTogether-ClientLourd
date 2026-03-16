using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Notice
{
    public int Id { get; set; }

    public string Comment { get; set; } = null!;

    public short Note { get; set; }

    public int UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
