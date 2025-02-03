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

        public HomeController(IMatchRepository matchRepository,ITicketRepository ticketRepository,IBookingRepository bookingRepository)
        {
            this._matchRepository = matchRepository;
            this._ticketRepository = ticketRepository;
            this._bookingRepository = bookingRepository;
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
                matchBookings[match.MatchId] = bookings ?? new List<Booking>(); // التأكد من أن القائمة ليست null
            }


            var homeVM = new HomeVM
            {
                Matches = matches,
                MatchBooking = matchBookings 
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
