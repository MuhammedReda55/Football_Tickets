using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Football_Tickets.Models;
using Football_Tickets.Models.ViewModels;
using Football_Tickets.Repository.IRepository;
using Football_Tickets.Utility;

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

    public async Task<IActionResult> Success()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["message"] = "حدث خطأ أثناء الدفع.";
            return RedirectToAction("Index", "Home");
        }

        var cart = cartRepository.Get(
            includeProps: [e => e.Match.HomeTeam, e => e.Match.AwayTeam, e => e.Match.Stadium],
            filter: e => e.ApplicationUserId == user.Id
        ).ToList();

        if (cart == null || !cart.Any())
        {
            TempData["message"] = "سلة التسوق فارغة.";
            return RedirectToAction("Index", "Home");
        }

        var firstItem = cart.First();

        // تحديد السعر بناءً على القسم المحجوز
        decimal ticketPrice = firstItem.section switch
        {
            "Section1" => firstItem.Match.Section1Price ?? 0,
            "Section2" => firstItem.Match.Section2Price ?? 0,
            "Section3" => firstItem.Match.Section3Price ?? 0,
            _ => 0
        };

        var ticketDetails = new TicketDetailsVM
        {
            UserName = user.UserName,
            Email = user.Email,
            HomeTeam = firstItem.Match.HomeTeam.TeamName,
            AwayTeam = firstItem.Match.AwayTeam.TeamName,
            StadiumName = firstItem.Match.Stadium.Name,
            Section = firstItem.section, // إضافة القسم الذي تم الحجز فيه
            Price = ticketPrice,
            SeatNumber=firstItem.SeatNumber,
            Count = cart.Sum(e => e.Count),
            TotalPrice = cart.Sum(e => ticketPrice * e.Count),
            Date = firstItem.Match.MatchDate
        };

        // تحويل البيانات إلى JSON (اختياري)
        var jsonTicketDetails = JsonConvert.SerializeObject(ticketDetails);

        // تحميل قالب البريد الإلكتروني
        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/TicketDetailsTemplate.html");
        string emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

        // استبدال البيانات في القالب
        var emailBody = emailTemplate
            .Replace("{{UserName}}", ticketDetails.UserName)
            .Replace("{{Email}}", ticketDetails.Email)
            .Replace("{{HomeTeam}}", ticketDetails.HomeTeam)
            .Replace("{{AwayTeam}}", ticketDetails.AwayTeam)
            .Replace("{{StadiumName}}", ticketDetails.StadiumName)
            .Replace("{{Section}}", ticketDetails.Section)
            .Replace("{{Price}}", ticketDetails.Price.ToString("F2"))
           .Replace("{{SeatNumber}}", ticketDetails.SeatNumber?.ToString() ?? "غير محدد")
            .Replace("{{Count}}", ticketDetails.Count.ToString())
            .Replace("{{TotalPrice}}", ticketDetails.TotalPrice.ToString("F2"))
            .Replace("{{Date}}", ticketDetails.Date.ToString("yyyy-MM-dd HH:mm"));

        // إرسال البريد الإلكتروني
        await emailSender.SendEmailAsync(user.Email, "تأكيد حجز التذكرة", emailBody);

        return View();
    }
    public IActionResult Cancel()
    {
        return View();
    }
    public IActionResult Ticket()
    {
        var userId = userManager.GetUserId(User);
        var cart = cartRepository.Get(
            includeProps: [e => e.Match.HomeTeam, e => e.Match.AwayTeam, e => e.Match.Stadium],
            filter: e => e.ApplicationUserId == userId
        );

        if (!cart.Any())
        {
            TempData["message"] = "لا توجد تذاكر في سلة المشتريات!";
            return RedirectToAction("Index", "Home");
        }

        var username = userManager.GetUserName(User);
        double totalPrice = cart.Sum(e =>
            e.section == "Section1" ? (double)e.Match.Section1Price * e.Count :
            e.section == "Section2" ? (double)e.Match.Section2Price * e.Count :
            (double)e.Match.Section3Price * e.Count
        );

        CartWithTotalPriceVM cartWithTotal = new CartWithTotalPriceVM()
        {
            Carts = cart.ToList(),
            UserName = username,
            TotalPrice = totalPrice
        };

        return View(cartWithTotal);
    }

}
