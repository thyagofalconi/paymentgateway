using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.API.Authentication;
using PaymentGateway.Domain.PaymentProcessing.Interfaces;
using PaymentGateway.Model.PaymentProcessing;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentProcessingController : ControllerBase
    {
        private readonly ILogger<PaymentProcessingController> _logger;

        private readonly IPaymentHandler _paymentHandler;

        public PaymentProcessingController(ILogger<PaymentProcessingController> logger, IPaymentHandler paymentHandler)
        {
            _logger = logger;
            _paymentHandler = paymentHandler;
        }

        [HttpPost]
        [Route("v1")]
        [BasicAuth("payment-processing")]
        public async Task<IActionResult> Post([FromBody] PaymentProcessingRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _paymentHandler.Handle(request);

                return Ok(result);
            }
            catch (PaymentProcessingException exception)
            {
                return BadRequest(new PaymentProcessingFailedResponse
                {
                    Success = false,
                    Message = $"An error has occurred. Details: {exception.Message}"
                });
            }
            catch (Exception exception)
            {
                return BadRequest(new PaymentProcessingFailedResponse
                {
                    Success = false,
                    Message = $"An unhandled error has occurred. Details: {exception.Message}"
                });
            }
        }
    }
}
