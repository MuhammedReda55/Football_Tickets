using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int MatchId { get; set; }

    public int SectionId { get; set; }

    public int? Seatnumber { get; set; }

    public int? StadiumId { get; set; }

    public decimal? SectionPrice { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Match Match { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Section Section { get; set; } = null!;

    public virtual Stadium Stadium { get; set; } = null!;
}
