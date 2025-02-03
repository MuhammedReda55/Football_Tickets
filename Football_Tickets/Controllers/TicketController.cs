using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Football_Tickets.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketController(ITicketRepository ticketRepository)
        {
            this._ticketRepository = ticketRepository;
        }
        public IActionResult Index()
        {
            var tickets = _ticketRepository.Get(includeProps: [e=>e.Match.HomeTeam,e=>e.Match.AwayTeam,e=>e.Stadium]).ToList();
            return View(tickets);
        }
        
        
    }
}
