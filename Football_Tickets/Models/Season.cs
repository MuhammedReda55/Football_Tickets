using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Football_Tickets.Models;

public partial class Season
{
    public int SeasonId { get; set; }
    [Required]
    public string SeasonName { get; set; } = null!;
    [Required]
    public DateOnly StartDate { get; set; }
    [Required]
    public DateOnly EndDate { get; set; }
    [Required]
    public decimal BasePrice { get; set; }
    [ValidateNever]
    public virtual ICollection<SeasonTicket> SeasonTickets { get; set; } = new List<SeasonTicket>();
}
