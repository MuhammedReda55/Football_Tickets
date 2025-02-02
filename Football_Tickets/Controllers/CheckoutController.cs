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

        var cartItems = cartRepository.Get(
            includeProps: [e => e.Match, e => e.Match.Stadium, e => e.Match.HomeTeam, e => e.Match.AwayTeam],
            filter: e => e.ApplicationUserId == user.Id
        ).ToList();

        if (cartItems == null || !cartItems.Any())
        {
            TempData["message"] = "سلة التسوق فارغة.";
            return RedirectToAction("Index", "Home");
        }

        foreach (var cart in cartItems)
        {
            // استخراج `SectionId` وسعر التذكرة بناءً على القسم
            int sectionId = cart.section switch
            {
                "Section1" => 1,
                "Section2" => 2,
                "Section3" => 3,
                _ => 0
            };

            decimal sectionPrice = cart.section switch
            {
                "Section1" => cart.Match.Section1Price ?? 0,
                "Section2" => cart.Match.Section2Price ?? 0,
                "Section3" => cart.Match.Section3Price ?? 0,
                _ => 0
            };

            // إنشاء وحفظ التذكرة `Ticket`
            var ticket = new Ticket
            {
                MatchId = cart.MatchId,
                SectionId = sectionId,
                Seatnumber = cart.SeatNumber,
                StadiumId = cart.Match.StadiumId,
                SectionPrice = sectionPrice
            };
            ticketRepository.Create(ticket);
            ticketRepository.Commit();

            // حفظ الحجز `Booking`
            var booking = new Booking
            {
                MatchId = cart.MatchId,
                TicketId = ticket.TicketId,
                Date = DateTime.Now
            };
            bookingRepository.Create(booking);
            bookingRepository.Commit();

            // حفظ الدفع `Payment`
            var payment = new Payment
            {
                TicketId = ticket.TicketId,
                Date = DateTime.Now,
                Method = "Credit Card",
                Amount = sectionPrice
            };
            paymentRepository.Create(payment);
            paymentRepository.Commit();
        }

        foreach (var cart in cartItems)
        {
            cartRepository.Delete(cart);
        }
        cartRepository.Commit();

        // Now prepare the email
        var firstItem = cartItems.First();
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
            Section = firstItem.section,
            Price = ticketPrice,
            SeatNumber = firstItem.SeatNumber,
            Count = cartItems.Sum(e => e.Count),
            TotalPrice = cartItems.Sum(e => ticketPrice * e.Count),
            Date = firstItem.Match.MatchDate
        };

        var jsonTicketDetails = JsonConvert.SerializeObject(ticketDetails);

        string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/templates/TicketDetailsTemplate.html");
        string emailTemplate = await System.IO.File.ReadAllTextAsync(templatePath);

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

        // Send the email to the user
        await emailSender.SendEmailAsync(user.Email, "تأكيد حجز التذكرة", emailBody);

        TempData["message"] = "تم الدفع بنجاح، وتم حفظ التذاكر.";
        //return RedirectToAction("Success");
        return View();
    }

    public IActionResult Cancel()
    {
        return View();
    }
    public IActionResult Ticket()
    {
        var userId = userManager.GetUserId(User);

        
        var tickets = ticketRepository.Get(
            includeProps: [e => e.Match, e => e.Match.Stadium, e => e.Match.HomeTeam, e => e.Match.AwayTeam],
            filter: e => e.Bookings.Any(b => b.MatchId == e.MatchId)
        );

        if (!tickets.Any())
        {
            TempData["message"] = "لا توجد تذاكر محجوزة!";
            return RedirectToAction("Index", "Home");
        }

        var username = userManager.GetUserName(User);

        double totalPrice = tickets.Sum(e => (double)(e.SectionPrice ?? 0));

         
        var ticketDetails = new TicketWithTotalPriceVM
        {
            Tickets = tickets.ToList(),
            UserName = username,
            TotalPrice = totalPrice
        };

        return View(ticketDetails);
    }


}
