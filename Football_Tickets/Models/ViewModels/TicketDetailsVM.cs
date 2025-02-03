namespace Football_Tickets.Models.ViewModels
{
    public class TicketDetailsVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string StadiumName { get; set; }
        public string Section { get; set; } 
        public int? SeatNumber { get; set; } 
        
        public decimal? Price { get; set; }
        public int Count { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateTime Date { get; set; }
    }
}
