using Football_Tickets.Models;
using Football_Tickets.Models.ViewModels;
using Football_Tickets.Repository;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Football_Tickets.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepository cartRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITicketRepository _ticketRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ISectionRepository _sectionRepository;

        public CartController(ICartRepository cartRepository, IMatchRepository matchRepository, UserManager<ApplicationUser> userManager,
            ITicketRepository ticketRepository,IBookingRepository bookingRepository,ISectionRepository sectionRepository)
        {
            this.cartRepository = cartRepository;
            this._matchRepository = matchRepository;
            this.userManager = userManager;
            this._ticketRepository = ticketRepository;
            this._bookingRepository = bookingRepository;
            this._sectionRepository = sectionRepository;
        }
        public IActionResult Index()
        {
            var userId = userManager.GetUserId(User);

            
            var expiredCarts = cartRepository.Get(filter: c => c.ExpireTime < DateTime.Now).ToList();
            foreach (var expiredCart in expiredCarts)
            {
                cartRepository.Delete(expiredCart);
            }
            cartRepository.Commit();

           
            var cart = cartRepository.Get(
                includeProps: [e => e.Match.AwayTeam, e => e.Match.HomeTeam, e => e.Match.Stadium],
                filter: e => e.ApplicationUserId == userId
            );

            double totalPrice = cart.Sum(e =>
                e.section == "Section1" ? (double)(e.Match.Section1Price ?? 0) * e.Count :
                e.section == "Section2" ? (double)(e.Match.Section2Price ?? 0) * e.Count :
                e.section == "Section3" ? (double)(e.Match.Section3Price ?? 0) * e.Count :
                0
            );

            CartWithTotalPriceVM cartWithTotal = new CartWithTotalPriceVM()
            {
                Carts = cart.ToList(),
                TotalPrice = totalPrice
            };

            return View(cartWithTotal);
        }



        public IActionResult AddToCart(int MatchId, int count = 1, int SeatNumber = 1, string section = "Section1")
        {
            var userId = userManager.GetUserId(User);
            if (userId != null)
            {
                var cart = cartRepository.GetOne(filter: c => c.ApplicationUserId == userId && c.MatchId == MatchId && c.SeatNumber == SeatNumber && c.section == section);
                if (cart != null)
                {
                    TempData["message"] = "هذا المقعد محجوز بالفعل لهذه المباراة، يرجى اختيار مقعد آخر.";
                    return RedirectToAction("Index", "Home");
                }

                var existingBooking = _bookingRepository.GetOne(filter: b => b.Ticket.MatchId == MatchId && b.Ticket.Seatnumber == SeatNumber && b.Ticket.Sections == section);

                if (existingBooking != null)
                {
                    TempData["message"] = "هذا المقعد محجوز بالفعل لهذه المباراة، يرجى اختيار مقعد آخر.";
                    return RedirectToAction("Index", "Home");
                }

                cartRepository.Create(new Cart
                {
                    ApplicationUserId = userId,
                    Count = count,
                    SeatNumber = SeatNumber,
                    section = section,
                    MatchId = MatchId,
                    Time = DateTime.Now,
                    ExpireTime = DateTime.Now.AddDays(1) 
                });

                cartRepository.Commit();

                TempData["message"] = "تمت إضافة التذكرة إلى سلتك بنجاح.";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login", "Account");
        }


        public IActionResult Increment(int MatchId)
        {
            var cart = cartRepository.GetOne(filter: e => e.ApplicationUserId == userManager.GetUserId(User) && e.MatchId == MatchId);

            cart.Count++;
            cartRepository.Commit();

            return RedirectToAction("Index");
        }
        public IActionResult Decreament(int MatchId)
        {

            var cart = cartRepository.GetOne(filter: e => e.ApplicationUserId == userManager.GetUserId(User) && e.MatchId == MatchId);

            var count = cart.Count--;
            if (count >= 1)
            {

                cartRepository.Commit();
            }


            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var cart = cartRepository.GetOne(e => e.MatchId == id);

            if (cart != null)
            {

                cartRepository.Delete(cart);
                cartRepository.Commit();
                TempData["message"] = "Delete Ticket successfuly";
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundSearch", "Home");
        }
        public IActionResult EditSeat(int MatchId)
        {
            var userId = userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItem = cartRepository.GetOne(filter: e => e.ApplicationUserId == userId && e.MatchId == MatchId);
            if (cartItem == null) return NotFound();

            return View(cartItem);
        }
        [HttpPost]
        public IActionResult EditSeat(Cart cart)
        {
            var userId = userManager.GetUserId(User);
            if (userId == null) return RedirectToAction("Login", "Account");

            var cartItem = cartRepository.GetOne(filter: e => e.ApplicationUserId == userId && e.MatchId == cart.MatchId);
            if (cartItem == null) return NotFound();

            
            var reservedSeats = cartRepository.Get(filter: e => e.MatchId == cart.MatchId
                                                            && e.SeatNumber == cart.SeatNumber
                                                            && e.section == cart.section).ToList();

            if (reservedSeats.Count > 0)
            {
                
                TempData["message"] = "المقعد في هذا الـ Section محجوز بالفعل.";
                return RedirectToAction("Index", "Cart");
            }

            
            cartItem.SeatNumber = cart.SeatNumber;
            cartItem.section = cart.section;
            cartRepository.Commit();

            TempData["message"] = "تم تعديل المقعد والـسيكشن بنجاح.";
            return RedirectToAction("Index", "Cart");
        }
        public IActionResult Pay()
        {
            var userId = userManager.GetUserId(User);
            if (userId == null)
            {
                TempData["message"] = "يجب تسجيل الدخول لإتمام عملية الدفع.";
                return RedirectToAction("Index", "Home");
            }

            var cartItems = cartRepository.Get(
                includeProps: [e => e.Match, e => e.Match.Stadium, e => e.Match.HomeTeam, e => e.Match.AwayTeam],
                filter: e => e.ApplicationUserId == userId
            ).ToList();

            if (!cartItems.Any())
            {
                TempData["message"] = "سلة التسوق فارغة.";
                return RedirectToAction("Index", "Home");
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Cancel",
            };

            foreach (var cart in cartItems)
            {
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

                // إنشاء التذكرة
                var ticket = new Ticket
                {
                    MatchId = cart.MatchId,
                    SectionId = sectionId,
                    Seatnumber = cart.SeatNumber,
                    StadiumId = cart.Match.StadiumId,
                    SectionPrice = sectionPrice,
                    Sections = cart.section
                };
                _ticketRepository.Create(ticket);
                _ticketRepository.Commit();

                // إنشاء الحجز
                var booking = new Booking
                {
                    MatchId = cart.MatchId,
                    ApplicationUserId = userId,
                    TicketId = ticket.TicketId,
                    Date = DateTime.Now
                };
                _bookingRepository.Create(booking);
                _bookingRepository.Commit();

                // إضافة بيانات التذكرة إلى Stripe
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"{cart.Match.HomeTeam.TeamName} vs {cart.Match.AwayTeam.TeamName} - {cart.section}",
                        },
                        UnitAmount = (long)(sectionPrice * 100),
                    },
                    Quantity = cart.Count,
                });
            }

            // حذف العناصر من سلة التسوق بعد حفظ التذاكر
            foreach (var cart in cartItems)
            {
                cartRepository.Delete(cart);
            }
            cartRepository.Commit();

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }





    }
}