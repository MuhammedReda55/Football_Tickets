using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string TeamName { get; set; } = null!;

    public string? LogoUrl { get; set; }

    public virtual ICollection<Match> MatchAwayTeams { get; set; } = new List<Match>();

    public virtual ICollection<Match> MatchHomeTeams { get; set; } = new List<Match>();

    public virtual ICollection<SeasonTicket> SeasonTickets { get; set; } = new List<SeasonTicket>();
}
