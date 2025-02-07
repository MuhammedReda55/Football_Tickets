namespace Football_Tickets.Models.ViewModels
{
    public class MatchDetailsVM
    {
        public Match Match { get; set; }
        public int BookedTickets { get; set; }
        public List<Booking> Bookings { get; set; }
    }
}
