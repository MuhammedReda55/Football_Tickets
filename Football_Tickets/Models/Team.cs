using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Football_Tickets.Models;

public partial class Team
{
    public int TeamId { get; set; }
    [Required]
    public string TeamName { get; set; } = null!;
    [ValidateNever]
    public string? LogoUrl { get; set; }
    [Required]
    public string? Description { get; set; }
    [ValidateNever]
    public int? StadiumId { get; set; } 

    public virtual Stadium? Stadium { get; set; }
    [ValidateNever]
    public virtual ICollection<Match> MatchAwayTeams { get; set; } = new List<Match>();
    [ValidateNever]
    public virtual ICollection<Match> MatchHomeTeams { get; set; } = new List<Match>();
    [ValidateNever]
    public virtual ICollection<SeasonTicket> SeasonTickets { get; set; } = new List<SeasonTicket>();
}
