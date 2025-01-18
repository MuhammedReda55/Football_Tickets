using Football_Tickets.Models.ViewModels;
using Football_Tickets.Models;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Football_Tickets.Utility;



namespace Football_Tickets.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IEmailSender emailSender;
        private readonly ICartRepository cartRepository;
        private readonly UserManager<ApplicationUser> userManager;


        public CheckoutController(IEmailSender emailSender, ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            this.emailSender = emailSender;
            this.cartRepository = cartRepository;
            this.userManager = userManager;

        }

        //public async Task<IActionResult>  Success()
        //{
        //    var user = await userManager.GetUserAsync(User);
        //    if (user != null)
        //    {
        //        await emailSender.SendEmailAsync(await userManager.GetEmailAsync(user), "success pay", $"<html><body> <h1>{cartRepository.Get(includeProps: [e=>e.Movie.Name])}</h1> </body></html>");
        //        return View();

        //    }
        //    TempData["message"] = "هناك خطا في الدفع";
        //    return RedirectToAction("Index", "Home");
        //}
        public async Task<IActionResult> Success()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {

                var cart = cartRepository.Get(
                    includeProps: [e => e.Match.Stadium,e=>e.Match.Tickets],
                    filter: e => e.ApplicationUserId == user.Id
                );

                if (cart != null && cart.Any())
                {
                    var ticketDetails = new TicketDetailsVM
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        HomeTeam = cart.First().Match.HomeTeam.TeamName,
                        AwayTeam = cart.First().Match.AwayTeam.TeamName,
                        StadiumName = cart.First().Match.Stadium.Name,
                        Location = cart.First().Match.Stadium.Location,
                        MatchDate = cart.First().Match.MatchDate,
                        MatchTime = cart.First().Match.MatchTime,
                        Price = (double)cart.First().Match.Tickets.FirstOrDefault().Price,
                        Count = cart.Sum(e => e.Count),
                        TotalPrice = (double)cart.Sum(e => e.Match.Tickets.FirstOrDefault().Price * e.Count),
                        Date = DateTime.Now
                    };

                    // تحويل البيانات إلى JSON
                    var jsonTicketDetails = JsonConvert.SerializeObject(ticketDetails);


                    string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/TicketDetailsTemplate.html");
                    string emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

                    var emailBody = emailTemplate
                        .Replace("{{UserName}}", ticketDetails.UserName)
                        .Replace("{{Email}}", ticketDetails.Email)
                        .Replace("{{HomeTeam}}", ticketDetails.HomeTeam)
                        .Replace("{{AwayTeam}}", ticketDetails.AwayTeam)
                        .Replace("{{StadiumName}}", ticketDetails.StadiumName)
                        .Replace("{{Location}}", ticketDetails.Location)
                        .Replace("{{MatchDate}}", ticketDetails.MatchDate.ToString("yyyy-MM-dd"))
                        .Replace("{{MatchTime}}", ticketDetails.MatchTime.ToString("hh\\:mm tt"))
                        .Replace("{{Price}}", ticketDetails.Price.ToString("F2"))
                        .Replace("{{Count}}", ticketDetails.Count.ToString())
                        .Replace("{{TotalPrice}}", ticketDetails.TotalPrice.ToString("F2"))
                        .Replace("{{Date}}", ticketDetails.Date.ToString("yyyy-MM-dd HH:mm"));


                    await emailSender.SendEmailAsync(user.Email, "Success", emailBody);
                }

                return View();
            }

            TempData["message"] = "هناك خطأ في الدفع";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Cancel()
        {
            return View();
        }
        public IActionResult ticket()
        {
            var cart = cartRepository.Get(includeProps: [e => e.Match.Stadium, e => e.Match.Tickets], 
                filter: e => e.ApplicationUserId == userManager.GetUserId(User));

            //ViewBag.Total = cart.Sum(e => e.Product.Price * e.Count);
            var username = userManager.GetUserName(User);
            CartWithTotalPriceVM cartWithTotal = new CartWithTotalPriceVM()
            {
                Carts = cart.ToList(),
                UserName = username,
                TotalPrice = (double)cart.Sum(e => e.Match.Tickets.FirstOrDefault().Price * e.Count)
            };


            return View(cartWithTotal);
        }
    }
}
