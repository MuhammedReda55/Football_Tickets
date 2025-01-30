using System.Diagnostics;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Football_Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMatchRepository _matchRepository;

        public HomeController(IMatchRepository matchRepository)
        {
            this._matchRepository = matchRepository;
        }

        public IActionResult Index()
        {
            var match = _matchRepository.Get(includeProps: [e=>e.Stadium, e=>e.HomeTeam, e=>e.AwayTeam
            ]).ToList();
            return View(match);
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
