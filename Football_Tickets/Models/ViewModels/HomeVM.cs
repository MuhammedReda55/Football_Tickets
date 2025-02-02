namespace Football_Tickets.Models.ViewModels
{
    public class HomeVM
    {
        public List<Match> Matches { get; set; } = new();
        public Dictionary<int, List<Ticket>> MatchTickets { get; set; } = new();
    }
}
