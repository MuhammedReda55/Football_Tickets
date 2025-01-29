using System;
using System.Collections.Generic;

namespace Football_Tickets.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int TicketId { get; set; }

    public DateTime Date { get; set; }

    public string? Method { get; set; }

    public decimal Amount { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
}
