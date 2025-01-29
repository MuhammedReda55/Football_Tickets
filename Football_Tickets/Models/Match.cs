using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Football_Tickets.Models;

public partial class Match
{
    public int MatchId { get; set; }
    [ValidateNever]
    public int? HomeTeamId { get; set; }
    [ValidateNever]
    public int? AwayTeamId { get; set; }
    [Required]
    //[MinLength(3, ErrorMessage = "Minimum length must be 3")]
    //[MaxLength(50, ErrorMessage = "Maximum length must be 20")]
    public DateTime MatchDate { get; set; }
    [ValidateNever]
    public int? StadiumId { get; set; }
    [Required]
    public decimal? Section1Price { get; set; }
    [Required]
    public decimal? Section2Price { get; set; }
    [Required]
    public decimal? Section3Price { get; set; }
    [ValidateNever]
    public virtual Team AwayTeam { get; set; } = null!;
    [ValidateNever]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    [ValidateNever]
    public virtual Team HomeTeam { get; set; } = null!;
    [ValidateNever]
    public virtual Stadium Stadium { get; set; } = null!;
    [ValidateNever]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
