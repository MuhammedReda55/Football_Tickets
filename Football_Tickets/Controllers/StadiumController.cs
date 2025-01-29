using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Football_Tickets.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class StadiumController : Controller
    {
        private readonly IStadiumRepository _stadiumRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ITeamRepository _teamRepository;

        public StadiumController(IStadiumRepository stadiumRepository,ISectionRepository sectionRepository,
            ITeamRepository teamRepository)
        {
            this._stadiumRepository = stadiumRepository;
            this._sectionRepository = sectionRepository;
            this._teamRepository = teamRepository;
        }
        public IActionResult Index()
        {
            var stadium = _stadiumRepository.Get(includeProps: [e=>e.Sections, e=>e.Teams]).ToList();
            return View(stadium);
        }
        public IActionResult Create()
        {
            
            var team = _teamRepository.Get().ToList();
            ViewData["team"] = team;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Stadium stadium)
        {

            ModelState.Remove("Matches");
            ModelState.Remove("Tickets");
            ModelState.Remove("Sections");
            ModelState.Remove("SelectedTeamId");
           
            if (ModelState.IsValid)
            {
                if (stadium.SelectedTeamId.HasValue)
                {
                    var team = _teamRepository.GetOne(filter:e=>e.TeamId==stadium.SelectedTeamId);
                    if (team != null)
                    {
                        stadium.Teams.Add(team);
                    }
                }
                _stadiumRepository.Create(stadium);
                _stadiumRepository.Commit();
                TempData["message"] = "Create Stadium successfuly";
                return RedirectToAction("Index");

            }
            return View(stadium);
        }
        
        
    }
}
