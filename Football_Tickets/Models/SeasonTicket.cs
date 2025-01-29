using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class SeasonTicket
{
    public int SeasonTicketId { get; set; }

    public int TeamId { get; set; }

    public int SeasonId { get; set; }

    public int SectionId { get; set; }

    public decimal? TotalPrice { get; set; }

    public virtual Season Season { get; set; } = null!;

    public virtual Section Section { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
