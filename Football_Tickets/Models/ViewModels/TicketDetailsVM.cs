namespace Football_Tickets.Models.ViewModels
{
    public class TicketDetailsVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string StadiumName { get; set; }
        public string Location { get; set; }
        public DateOnly MatchDate { get; set; }
        public TimeOnly MatchTime { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
        public double TotalPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
