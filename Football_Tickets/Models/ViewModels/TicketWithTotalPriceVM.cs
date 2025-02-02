namespace Football_Tickets.Models.ViewModels
{
    public class TicketWithTotalPriceVM
    {
        public List<Ticket> Tickets { get; set; }
        public string? UserName { get; set; }
        public double TotalPrice { get; set; }
    }
}
