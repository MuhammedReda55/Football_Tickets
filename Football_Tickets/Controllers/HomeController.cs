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
        private readonly IBookingRepository _bookingRepository;
        private readonly ITeamRepository _teamRepository;

        public HomeController(IMatchRepository matchRepository,ITicketRepository ticketRepository,IBookingRepository bookingRepository,
            ITeamRepository teamRepository)
        {
            this._matchRepository = matchRepository;
            this._ticketRepository = ticketRepository;
            this._bookingRepository = bookingRepository;
            this._teamRepository = teamRepository;
        }

        public IActionResult Index()
        {
            
            var matches = _matchRepository.Get(
                includeProps: [e => e.Stadium, e => e.HomeTeam, e => e.AwayTeam]
            ).ToList();

            var matchBookings = new Dictionary<int, List<Booking>>();


            foreach (var match in matches)
            {
                var bookings = _bookingRepository.Get(filter: t => t.MatchId == match.MatchId, includeProps: [e=>e.Ticket]).ToList();
                matchBookings[match.MatchId] = bookings ?? new List<Booking>(); 
            }


            var homeVM = new HomeVM
            {
                Matches = matches,
                MatchBooking = matchBookings 
            };

            return View(homeVM);
        }
        public IActionResult Details(int id)
        {
            var match = _matchRepository.GetOne(
                filter: m => m.MatchId == id,
                includeProps: [m => m.Stadium, m => m.HomeTeam, m => m.AwayTeam, m => m.Bookings]
            );

            if (match == null)
            {
                return NotFound();
            }

            var bookings = _bookingRepository.Get(filter: t => t.MatchId == id, includeProps: [e => e.Ticket]).ToList();

            var matchDetailsVM = new MatchDetailsVM
            {
                Match = match,
                BookedTickets = bookings.Count,
                Bookings = bookings
            };

            return View(matchDetailsVM);
        }

        public IActionResult Search(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return RedirectToAction("Index");
            }

            var matches = _matchRepository.Get(filter: m =>
                m.HomeTeam.TeamName.Contains(name) || m.AwayTeam.TeamName.Contains(name),
                includeProps: [m => m.Stadium, m => m.HomeTeam, m => m.AwayTeam,e=>e.Bookings]
            ).ToList();

            if (!matches.Any())
            {
                return RedirectToAction("NotFoundSearch");
            }

            var matchBookings = new Dictionary<int, List<Booking>>();
            foreach (var match in matches)
            {
                var bookings = _bookingRepository.Get(filter: t => t.MatchId == match.MatchId, includeProps: [e => e.Ticket]).ToList();
                matchBookings[match.MatchId] = bookings ?? new List<Booking>();
            }

            var homeVM = new HomeVM
            {
                Matches = matches,
                MatchBooking = matchBookings
            };

            return View("Search", homeVM);
        }






        public IActionResult NotFoundSearch()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Newpage()
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
