using Football_Tickets.Data;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;

namespace Football_Tickets.Repository
{
    public class MatchRepository : Repository<Match>, IMatchRepository
    {
        public MatchRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
