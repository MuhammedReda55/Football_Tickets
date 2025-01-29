using Football_Tickets.Data;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;

namespace Football_Tickets.Repository
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
