using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class Match
{
    public int MatchId { get; set; }

    public int HomeTeamId { get; set; }

    public int AwayTeamId { get; set; }

    public DateOnly MatchDate { get; set; }

    public int StadiumId { get; set; }

    public TimeOnly MatchTime { get; set; }

    public virtual Team AwayTeam { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Team HomeTeam { get; set; } = null!;

    public virtual Stadium Stadium { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
