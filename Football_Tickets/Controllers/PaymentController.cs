using Football_Tickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace Football_Tickets.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            this._paymentRepository = paymentRepository;
        }
        public IActionResult Index()
        {
            var payment = _paymentRepository.Get(includeProps: [e => e.Ticket]).ToList();
            return View(payment);
        }
    }
}
