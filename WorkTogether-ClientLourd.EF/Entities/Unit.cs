using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Unit
{
    public int Id { get; set; }

    public int Position { get; set; }

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Type { get; set; }

    public int? PowerConsumption { get; set; }

    public double? Temperature { get; set; }

    public double? NetworkThroughput { get; set; }

    public int BayId { get; set; }

    public virtual Bay Bay { get; set; } = null!;

    public virtual ICollection<Intervention> Interventions { get; set; } = new List<Intervention>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
