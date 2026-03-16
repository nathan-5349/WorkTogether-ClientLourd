using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Offer
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int NbUnit { get; set; }

    public int Price { get; set; }

    public int Version { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
