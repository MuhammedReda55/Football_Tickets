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
using Football_Tickets.Repository;

public class CheckoutController : Controller
{
    private readonly IEmailSender emailSender;
    private readonly ICartRepository cartRepository;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly ITicketRepository ticketRepository;
    private readonly IBookingRepository bookingRepository;
    private readonly IPaymentRepository paymentRepository;

    public CheckoutController(IEmailSender emailSender, ICartRepository cartRepository, UserManager<ApplicationUser> userManager,
        ITicketRepository ticketRepository,IBookingRepository bookingRepository,IPaymentRepository paymentRepository)
    {
        this.emailSender = emailSender;
        this.cartRepository = cartRepository;
        this.userManager = userManager;
        this.ticketRepository = ticketRepository;
        this.bookingRepository = bookingRepository;
        this.paymentRepository = paymentRepository;
    }

    public async Task<IActionResult> Success()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            TempData["message"] = "يجب تسجيل الدخول لإتمام عملية الدفع.";
            return RedirectToAction("Index", "Home");
        }

        var tickets = ticketRepository.Get(
            includeProps: [e => e.Match, e => e.Match.Stadium, e => e.Match.HomeTeam, e => e.Match.AwayTeam],
            filter: e => e.MatchId != null 
        ).ToList();

        if (!tickets.Any())
        {
            TempData["message"] = "لم يتم العثور على تذاكر.";
            return RedirectToAction("Index", "Home");
        }

        foreach (var ticket in tickets)
        {
            // إنشاء سجل الدفع
            var payment = new Payment
            {
                TicketId = ticket.TicketId,
                Date = DateTime.Now,
                Method = "Credit Card",
                Amount = ticket.SectionPrice
            };
            paymentRepository.Create(payment);
            paymentRepository.Commit();
        }

        
        var firstTicket = tickets.First();

        var ticketDetails = new TicketDetailsVM
        {
            UserName = user.UserName,
            Email = user.Email,
            HomeTeam = firstTicket.Match.HomeTeam.TeamName,
            AwayTeam = firstTicket.Match.AwayTeam.TeamName,
            StadiumName = firstTicket.Match.Stadium.Name,
            Section = firstTicket.Sections,
            Price = firstTicket.SectionPrice,
            SeatNumber = firstTicket.Seatnumber,
            Count = tickets.Count,
            TotalPrice = tickets.Sum(e => e.SectionPrice),
            Date = firstTicket.Match.MatchDate
        };

        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/TicketDetailsTemplate.html");
        string emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

        var emailBody = emailTemplate
            .Replace("{{UserName}}", ticketDetails.UserName)
            .Replace("{{Email}}", ticketDetails.Email)
            .Replace("{{HomeTeam}}", ticketDetails.HomeTeam)
            .Replace("{{AwayTeam}}", ticketDetails.AwayTeam)
            .Replace("{{StadiumName}}", ticketDetails.StadiumName)
            .Replace("{{Section}}", ticketDetails.Section)
            .Replace("{{Price}}", ticketDetails.Price?.ToString("F2") ?? "0.00")

            .Replace("{{SeatNumber}}", ticketDetails.SeatNumber?.ToString() ?? "غير محدد")
            .Replace("{{Count}}", ticketDetails.Count.ToString())
            .Replace("{{TotalPrice}}", ticketDetails.TotalPrice?.ToString("F2") ?? "0.00")
            .Replace("{{Date}}", ticketDetails.Date.ToString("yyyy-MM-dd HH:mm"));

        // إرسال الإيميل للمستخدم
        await emailSender.SendEmailAsync(user.Email, "تأكيد حجز التذكرة", emailBody);

        TempData["message"] = "تم الدفع بنجاح، وتم تأكيد الحجز.";
        return View();
    }

    public IActionResult Cancel()
    {
        return View();
    }
    public IActionResult Ticket()
    {
        var userId = userManager.GetUserId(User);

        
        var lastBooking = bookingRepository.Get(
            includeProps: [e => e.Ticket, e => e.Ticket.Match, e => e.Ticket.Match.Stadium, e => e.Ticket.Match.HomeTeam, e => e.Ticket.Match.AwayTeam],
            filter: e => e.ApplicationUserId == userId
        ).OrderByDescending(e => e.Date).FirstOrDefault();

        if (lastBooking == null)
        {
            TempData["message"] = "لم يتم العثور على أي حجوزات!";
            return RedirectToAction("Index", "Home");
        }

        var ticketDetails = new TicketWithTotalPriceVM
        {
            Tickets = new List<Ticket> { lastBooking.Ticket },
            UserName = userManager.GetUserName(User),
            TotalPrice = (double)(lastBooking.Ticket.SectionPrice ?? 0)
        };

        return View(ticketDetails);
    }




}
