using System.Diagnostics;
using Football_Tickets.Models;
using Football_Tickets.Models.ViewModels;
using Football_Tickets.Repository;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Football_Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITicketRepository _ticketRepository;

        public HomeController(IMatchRepository matchRepository,ITicketRepository ticketRepository)
        {
            this._matchRepository = matchRepository;
            this._ticketRepository = ticketRepository;
        }

        public IActionResult Index()
        {
            var matches = _matchRepository.Get(
                includeProps: [e => e.Stadium, e => e.HomeTeam, e => e.AwayTeam]
            ).ToList();

            var matchTickets = new Dictionary<int, List<Ticket>>();

            foreach (var match in matches)
            {
                var tickets = _ticketRepository.Get(filter: t => t.MatchId == match.MatchId).ToList();
                matchTickets[match.MatchId] = tickets;
            }

            var homeVM = new HomeVM
            {
                Matches = matches,
                MatchTickets = matchTickets
            };

            return View(homeVM);
        }


        public IActionResult NotFoundSearch()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
