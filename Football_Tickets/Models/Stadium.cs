using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Football_Tickets.Models;

public partial class Stadium
{
   
    public int StadiumId { get; set; }
    [Required]
    public string Name { get; set; } = null!;
    [Required]
    public string? Location { get; set; }
    [Required]
    public int? Capacity { get; set; }
    [ValidateNever]
    public int? SelectedTeamId { get; set; }

    [ValidateNever]
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    [ValidateNever]
    public virtual ICollection<Section> Sections { get; set; } = new List<Section>();
    [ValidateNever]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    [Required]
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
