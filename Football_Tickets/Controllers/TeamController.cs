using Football_Tickets.Models;
using Football_Tickets.Repository;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Football_Tickets.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class TeamController : Controller
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IStadiumRepository _stadiumRepository;

        public TeamController(ITeamRepository teamRepository,IStadiumRepository stadiumRepository)
        {
            this._teamRepository = teamRepository;
            this._stadiumRepository = stadiumRepository;
        }
        public IActionResult Index()
        { 
            var team = _teamRepository.Get(includeProps: [e=>e.Stadium]).ToList();
            return View(team);
        }
        public IActionResult Create()
        {
            
            var team = _stadiumRepository.Get().ToList();
            ViewData["team"] = team;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Team team, IFormFile file)
        {
            ModelState.Remove("LogoUrl");
            ModelState.Remove("StadiumId");
            ModelState.Remove("MatchAwayTeams");
            ModelState.Remove("MatchHomeTeams");
            ModelState.Remove("SeasonTickets");

            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Generate unique file name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Save file in wwwroot
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logo_Teams", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Save file name in database
                    team.LogoUrl = fileName;
                }

                

                _teamRepository.Create(team);
                _teamRepository.Commit();

                TempData["message"] = "Team added successfully!";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
        public IActionResult Edit(int TeamID)
        {
            var stadium = _stadiumRepository.Get().ToList();
            ViewData["stadium"] = stadium;

            var team = _teamRepository.GetOne(e => e.TeamId == TeamID);
            if (team != null)
            {
                return View(team);
            }
            return RedirectToAction("NotFoundSearch", "Home");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Team team, IFormFile? file)
        {
            ModelState.Remove("LogoUrl");
            ModelState.Remove("StadiumId");
            ModelState.Remove("MatchAwayTeams");
            ModelState.Remove("MatchHomeTeams");
            ModelState.Remove("SeasonTickets");


            var oldTeam = _teamRepository.GetOne(filter: e => e.TeamId == team.TeamId, tracked: false);
            if (ModelState.IsValid)
            {
                if (file != null && file.Length > 0)
                {
                    // Generate new file name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                    // Path to save file
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logo_Teams", fileName);

                    // Save new file
                    using (var stream = System.IO.File.Create(filePath))
                    {
                        file.CopyTo(stream);
                    }

                    // Delete old file
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Logo_Teams", oldTeam.LogoUrl);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        try
                        {
                            var fileInfo = new FileInfo(oldFilePath);

                            // تحقق من خاصية Read-Only
                            if (fileInfo.IsReadOnly)
                            {
                                fileInfo.IsReadOnly = false;
                            }

                            // حذف الملف
                            System.IO.File.Delete(oldFilePath);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Console.WriteLine($"Error deleting file: {ex.Message}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                        }
                    }


                    team.LogoUrl = fileName;
                }
                else
                {
                    team.LogoUrl = oldTeam.LogoUrl;
                }
                


                _teamRepository.Alter(team);
                _teamRepository.Commit();

                TempData["message"] = "Update Team successfully";
                return RedirectToAction("Index");
            }

            team.LogoUrl = oldTeam.LogoUrl;
            return View(team);
        }
        [HttpGet]
        public IActionResult Delete(int TeamId)
        {
            var team = _teamRepository.GetOne(e => e.TeamId == TeamId);
            if (team != null)
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Logo_Teams", team.LogoUrl);

                try
                {
                    if (System.IO.File.Exists(oldPath))
                    {
                        var fileInfo = new FileInfo(oldPath);

                        // إزالة خاصية Read-Only إذا كانت موجودة
                        if (fileInfo.IsReadOnly)
                        {
                            fileInfo.IsReadOnly = false;
                        }

                        // حذف الملف
                        System.IO.File.Delete(oldPath);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }

                _teamRepository.Delete(team);
                _teamRepository.Commit();

                TempData["message"] = "Delete Team successfully";
                return RedirectToAction("Index");
            }

            return RedirectToAction("NotFoundSearch", "Home");
        }


    }
}
