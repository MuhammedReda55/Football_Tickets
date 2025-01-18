using Football_Tickets.Data;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;

namespace Football_Tickets.Repository
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
