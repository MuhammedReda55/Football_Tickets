using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int MatchId { get; set; }

    public int TicketId { get; set; }

    public DateTime Date { get; set; }

    public virtual Match Match { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
