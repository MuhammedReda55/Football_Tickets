using Football_Tickets.Models;
using Football_Tickets.Models.ViewModels;
using Football_Tickets.Repository;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Football_Tickets.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepository cartRepository;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(ICartRepository cartRepository, UserManager<ApplicationUser> userManager)
        {
            this.cartRepository = cartRepository;
            this.userManager = userManager;
        }
        public IActionResult Index(int MatchId, string section = "Section1")
        {
            
            var cart = cartRepository.Get(
                includeProps: [e=>e.Match.AwayTeam,e=>e.Match.HomeTeam,e=>e.Match.Stadium], // Include Match for navigation properties
                filter: e => e.ApplicationUserId == userManager.GetUserId(User) && e.MatchId==MatchId
            );

            
            double totalPrice = section switch
            {
                "Section1" => (double)cart.Sum(e => (e.Match.Section1Price ?? 0) * e.Count),
                "Section2" => (double)cart.Sum(e => (e.Match.Section2Price ?? 0) * e.Count),
                "Section3" => (double)cart.Sum(e => (e.Match.Section3Price ?? 0) * e.Count),
                _ => 0
            };

           
            CartWithTotalPriceVM cartWithTotal = new CartWithTotalPriceVM()
            {
                Carts = cart.ToList(),
                TotalPrice = totalPrice,
                SelectedSection = section 
            };

            return View(cartWithTotal);
        }
        public IActionResult AddToCart(int count, int MatchId, int SeatNumber)
        {
            var userId = userManager.GetUserId(User);
            if (userId != null)
            {
                var cartInBD = cartRepository.GetOne(filter: e => e.ApplicationUserId == userId && e.MatchId == MatchId);

                if (cartInBD != null)
                {
                    cartInBD.Count += count;
                    cartRepository.Commit();
                }

                else
                {
                    cartRepository.Create(new()
                    {
                        ApplicationUserId = userId,
                        Count = count,
                        MatchId = MatchId,
                        SeatNumber = SeatNumber,
                        Time = DateTime.Now
                    });
                    cartRepository.Commit();

                }

                TempData["message"] = "تم اضافة تذكرة الفليم الي السلة ";
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


    }
}
