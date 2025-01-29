using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football_Tickets.Models
{
    [PrimaryKey("ApplicationUserId", "MatchId")]
    public class Cart
    {
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int MatchId { get; set; }
        [ForeignKey("MatchId")]
        public Match Match { get; set; }

        public int Count { get; set; }
        public int? SeatNumber { get; set; }
        public DateTime Time { get; set; }
    }
}
