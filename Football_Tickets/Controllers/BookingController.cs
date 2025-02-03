using Football_Tickets.Repository;
using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Football_Tickets.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ITicketRepository _ticketRepository;

        public BookingController(IBookingRepository bookingRepository,ITicketRepository ticketRepository)
        {
            this._bookingRepository = bookingRepository;
            this._ticketRepository = ticketRepository;
        }
        public IActionResult Index()
        {
            var booking = _bookingRepository.Get(includeProps: [e=>e.Match.HomeTeam,
                e=>e.Match.AwayTeam,e=>e.Ticket]).ToList();
            return View(booking);
        }
        [HttpGet]
        public IActionResult Cancel(int id)
        {
            var booking = _bookingRepository.GetOne(filter: b => b.BookingId == id, includeProps: [b => b.Ticket]);

            if (booking == null)
            {
                TempData["message"] = "الحجز غير موجود!";
                return RedirectToAction("Index");
            }

            
            _bookingRepository.Delete(booking);
            _bookingRepository.Commit();


            TempData["message"] = "تم إلغاء الحجز بنجاح!";
            return RedirectToAction("Index");
        }


    }
}
