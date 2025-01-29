using Football_Tickets.Data;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Football_Tickets.Repository
{
    public class StadiumRepository : Repository<Stadium>, IStadiumRepository
    {
        public StadiumRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        


    }
}
