using AcquiringBank.API.Fake.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AcquiringBank.API.Fake.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SuccessPaymentController : ControllerBase
    {
        private readonly ILogger<SuccessPaymentController> _logger;

        public SuccessPaymentController(ILogger<SuccessPaymentController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public PaymentResponse Post([FromBody] PaymentRequest request)
        {
            return new PaymentResponse
            {
                Success = true,
                TransactionId = Guid.NewGuid()
            };
        }
    }
}
