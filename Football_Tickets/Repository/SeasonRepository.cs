using Football_Tickets.Data;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;

namespace Football_Tickets.Repository
{
    public class SeasonRepository : Repository<Season>, ISeasonRepository

    {
        public SeasonRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
