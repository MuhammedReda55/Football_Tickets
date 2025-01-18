using Football_Tickets.Models.ViewModels;
using Football_Tickets.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Football_Tickets.Repository.IRepository;

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
        public IActionResult Index()
        {
            var user = userManager.GetUserId(User);
            var cart = cartRepository.Get(includeProps: [e => e.Match, e=>e.Match.Stadium,
                e=>e.Match.Tickets,e=>e.Match.Stadium.Sections], filter: e => e.ApplicationUserId == user).ToList();
            CartWithTotalPriceVM cartWithTotal = new CartWithTotalPriceVM()
            {
                Carts = cart,
                TotalPrice = cart
                  .Where(e => e.Match.Tickets.Any()) 
                   .Sum(e => (double)(e.Match.Stadium.Sections.FirstOrDefault()?.Price ?? 0) * e.Count)
            };

            return View(cartWithTotal);
            

            //var cart = cartRepository.Get(
            //    includeProps: [e => e.Match.Tickets], 
            //    filter: e => e.ApplicationUserId == userId
            //);
            //var cartList = cartRepository.Get().ToList();


            //double totalPrice = cartList
            //    .Where(e => e.Match.Tickets.Any()) 
            //    .Sum(e => (double)(e.Match.Tickets.FirstOrDefault()?.Price ?? 0) * e.Count);

            //CartWithTotalPriceVM cartWithTotal = new CartWithTotalPriceVM()
            //{
            //    Carts = cart.ToList(),
            //    TotalPrice = totalPrice
            //};

            //return View(cartWithTotal);
        }
        public IActionResult AddToCart(int MatchId, int count)
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
                        Time = DateTime.Now
                    });

                }

                TempData["message"] = "تم اضافة تذكرة الماتش الي السلة ";
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
                TempData["message"] = "Delete Ticket successfuly";
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFoundSearch", "Home");
        }

    }
}
