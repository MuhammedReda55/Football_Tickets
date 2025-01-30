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

        public CartController(ICartRepository cartRepository, IMatchRepository matchRepository, UserManager<ApplicationUser> userManager)
        {
            this.cartRepository = cartRepository;
            this._matchRepository = matchRepository;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {

            var cart = cartRepository.Get(
                includeProps: [e => e.Match.AwayTeam, e => e.Match.HomeTeam, e => e.Match.Stadium], // Include Match for navigation properties
                filter: e => e.ApplicationUserId == userManager.GetUserId(User)
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
                // Check if the user has already booked a ticket for the selected match
                var cart = cartRepository.GetOne(filter: e => e.ApplicationUserId == userId && e.MatchId == MatchId);
                if (cart != null)
                {
                    TempData["message"] = "You have already booked a ticket for this match.";
                    return RedirectToAction("Index", "Home");
                }

                // Check if the seat is already booked in the same section
                var seatBooked = cartRepository.Get(filter: e => e.MatchId == MatchId && e.SeatNumber == SeatNumber && e.section == section).Any();
                if (seatBooked)
                {
                    TempData["message"] = "This seat in the selected section is already booked, please choose a different seat or section.";
                    return RedirectToAction("Index", "Home");
                }

                // Check  stadium capacity 
                var totalSeats = cartRepository.Get(filter: e => e.MatchId == MatchId, includeProps: [e => e.Match.Stadium]).Sum(e => e.Count);
                var match = _matchRepository.GetOne(m => m.MatchId == MatchId, includeProps: [e => e.Stadium]);
                if (totalSeats + count > match.Stadium.Capacity)
                {
                    TempData["message"] = "The stadium is at full capacity, no more seats available.";
                    return RedirectToAction("Index", "Home");
                }

                
                cartRepository.Create(new Cart
                {
                    ApplicationUserId = userId,
                    Count = count,
                    SeatNumber = SeatNumber,
                    section = section,
                    MatchId = MatchId,
                    Time = DateTime.Now
                });

                cartRepository.Commit();

                TempData["message"] = "Ticket successfully added to your cart.";
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
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Checkout/Cancel",
            };

            
            var cart = cartRepository.Get(includeProps: [e => e.Match.HomeTeam,e=>e.Match.AwayTeam], filter: e => e.ApplicationUserId == userManager.GetUserId(User)).ToList();

            foreach (var item in cart)
            {
                
                decimal ticketPrice = item.section switch
                {
                    "Section1" => item.Match.Section1Price ?? 0,
                    "Section2" => item.Match.Section2Price ?? 0,
                    "Section3" => item.Match.Section3Price ?? 0,
                    _ => 0 
                };

                
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp", 
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"{item.Match.HomeTeam.TeamName} vs {item.Match.AwayTeam.TeamName} - {item.section}", // اسم المباراة مع الـ section
                        },
                        UnitAmount = (long)(ticketPrice * 100), 
                    },
                    Quantity = item.Count, // عدد التذاكر
                });
            }

            var service = new SessionService();
            var session = service.Create(options); 
            return Redirect(session.Url);
        }




    }
}