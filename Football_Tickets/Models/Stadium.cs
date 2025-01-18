using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class Stadium
{
    public int StadiumId { get; set; }

    public string Name { get; set; } = null!;

    public string? Location { get; set; }

    public int? Capacity { get; set; }

    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
