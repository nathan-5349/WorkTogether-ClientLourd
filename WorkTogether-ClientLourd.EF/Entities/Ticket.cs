using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Ticket
{
    public int Id { get; set; }

    public string Subject { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime OpenDate { get; set; }

    public DateTime? CloseDate { get; set; }

    public string Status { get; set; } = null!;

    public int CustomerId { get; set; }

    public int TechniciansId { get; set; }

    public int SupportId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Support Support { get; set; } = null!;

    public virtual Technician Technicians { get; set; } = null!;
}
