using System;
using System.Collections.Generic;

namespace WorkTogether_ClientLourd.EF.Entities;

public partial class Reservation
{
    public int Id { get; set; }

    public DateTime BeginDate { get; set; }

    public DateTime FinishDate { get; set; }

    public string Status { get; set; } = null!;

    public int? ReservationOffer { get; set; }

    public int CustomerId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Offer? ReservationOfferNavigation { get; set; }

    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
