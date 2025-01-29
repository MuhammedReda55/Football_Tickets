using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Football_Tickets.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class SeasonController : Controller
    {
        private readonly ISeasonRepository _seasonRepository;

        public SeasonController(ISeasonRepository seasonRepository)
        {
            this._seasonRepository = seasonRepository;
        }
        public IActionResult Index()
        {
            var season = _seasonRepository.Get();
            return View(season.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Season season)
        {
            ModelState.Remove("SeasonTickets");
            if (ModelState.IsValid)
            {
                _seasonRepository.Create(season);
                _seasonRepository.Commit();
                TempData["message"] = "Create Season successfuly";
                return RedirectToAction("Index");
            }

            return View(season);
        }
        public IActionResult Edit(int SeasonId)
        {
            var season = _seasonRepository.GetOne(filter: e => e.SeasonId == SeasonId);
            return View(season);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Season season)
        {
            ModelState.Remove("SeasonTickets");
            if (ModelState.IsValid)
            {
                _seasonRepository.Alter(season);
                _seasonRepository.Commit();
                TempData["message"] = "Update Season successfuly";
                return RedirectToAction("Index");
            }

            return View(season);
        }
        [HttpPost]
        public IActionResult Delete(int SeasonId)
        {
            var season = _seasonRepository.GetOne(filter: e => e.SeasonId == SeasonId);
            if(season != null)
            {
                _seasonRepository.Delete(season);
                _seasonRepository.Commit();
                TempData["message"] = "Delete Season successfuly";
                return RedirectToAction("Index");
            }
            return View(season);
        }
    }
}
