using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class Season
{
    public int SeasonId { get; set; }

    public string SeasonName { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public decimal BasePrice { get; set; }

    public virtual ICollection<SeasonTicket> SeasonTickets { get; set; } = new List<SeasonTicket>();
}
