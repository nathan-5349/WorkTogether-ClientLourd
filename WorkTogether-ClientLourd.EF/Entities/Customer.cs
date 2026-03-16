using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Customer
{
    public string Adress { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string InvoiceAdress { get; set; } = null!;

    public int Id { get; set; }

    public virtual User IdNavigation { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
