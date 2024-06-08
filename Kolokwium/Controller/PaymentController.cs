using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kolokwium.Database;
using Kolokwium.Model;
using Kolokwium.Service;

namespace Kolokwium.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddPayment(int clientId, int subscriptionId, decimal amount)
        {
            var paymentId = await _service.AddPaymentAsync(clientId, subscriptionId, amount);
            if (paymentId == null)
            {
                return BadRequest("Payment wasn't processed");
            }

            return Ok(paymentId);
        }
    }
}