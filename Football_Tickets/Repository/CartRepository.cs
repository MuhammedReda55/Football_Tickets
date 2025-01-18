using Football_Tickets.Data;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;

namespace Football_Tickets.Repository
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
