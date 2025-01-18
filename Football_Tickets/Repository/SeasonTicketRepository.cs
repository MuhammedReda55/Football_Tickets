using Football_Tickets.Data;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;

namespace Football_Tickets.Repository
{
    public class SeasonTicketRepository : Repository<SeasonTicket>, ISeasonTicketRepository
    {
        public SeasonTicketRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
