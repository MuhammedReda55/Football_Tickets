using Football_Tickets.Models;
using Football_Tickets.Repository;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace Football_Tickets.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class MatchController : Controller
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IStadiumRepository _stadiumRepository;

        public MatchController(IMatchRepository matchRepository, ITeamRepository teamRepository,
            IStadiumRepository stadiumRepository)
        {
            this._matchRepository = matchRepository;
            this._teamRepository = teamRepository;
            this._stadiumRepository = stadiumRepository;
        }
        public IActionResult Index()
        {
            var match = _matchRepository.Get(includeProps: [e => e.HomeTeam, e => e.AwayTeam, e => e.Stadium]);
            return View(match);
        }

        public IActionResult DetailsTeam(int TeamId)
        {
            var team = _teamRepository.GetOne(filter: e => e.TeamId == TeamId,
                includeProps: [e => e.Stadium]);
            return View(team);
        }
        public IActionResult Create()
        {
            var team = _teamRepository.Get().ToList();
            ViewData["team"] = team;

            var stadium = _stadiumRepository.Get().ToList();
            ViewData["stadium"] = stadium;

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Match match)
        {
            ModelState.Remove("Bookings");
            ModelState.Remove("Tickets");
            ModelState.Remove("HomeTeamId");
            ModelState.Remove("AwayTeamId");
            ModelState.Remove("AwayTeam");
            ModelState.Remove("HomeTeam");
            ModelState.Remove("Stadium");



            if (ModelState.IsValid)
            {

                _matchRepository.Create(match);
                _matchRepository.Commit();
                TempData["message"] = "Add Match successfuly";
                return RedirectToAction("Index");
            }
            return View(match);
        }
        public IActionResult Edit(int MatchId)
        {
            var team = _teamRepository.Get().ToList();
            ViewData["team"] = team;

            var stadium = _stadiumRepository.Get().ToList();
            ViewData["stadium"] = stadium;

            var match = _matchRepository.GetOne(filter: e => e.MatchId == MatchId, includeProps: [e=>e.HomeTeam,
            e=>e.AwayTeam,e=>e.Stadium]);
            return View(match);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Match match)
        {
            ModelState.Remove("Bookings");
            ModelState.Remove("Tickets");
            ModelState.Remove("HomeTeamId");
            ModelState.Remove("AwayTeamId");
            ModelState.Remove("AwayTeam");
            ModelState.Remove("HomeTeam");
            ModelState.Remove("Stadium");

            if (ModelState.IsValid)
            {
                _matchRepository.Alter(match);
                _matchRepository.Commit();
                TempData["message"] = "Update Match successfuly";
                return RedirectToAction("Index");

            }
            return View(match);
        }
        [HttpPost]
        public IActionResult Delete(int MatchId)
        {
            var match = _matchRepository.GetOne(filter: e => e.MatchId == MatchId);
            if(match != null)
            {
                _matchRepository.Delete(match);
                _matchRepository.Commit();
                TempData["message"] = "Delete Match successfuly";
                return RedirectToAction("Index");
            }
            return View(match);
        }

    }
}
