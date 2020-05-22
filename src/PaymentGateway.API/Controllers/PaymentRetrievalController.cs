using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PaymentGateway.API.Authentication;
using PaymentGateway.Domain.PaymentRepository.Interfaces;
using PaymentGateway.Model.PaymentRepository;
using PaymentGateway.Model.PaymentRetrieval;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentRetrievalController : ControllerBase
    {
        private readonly ILogger<PaymentRetrievalController> _logger;

        private readonly IPaymentRepository _paymentRepository;

        public PaymentRetrievalController(ILogger<PaymentRetrievalController> logger, IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _paymentRepository = paymentRepository;
        }

        [HttpGet]
        [Route("v1")]
        [BasicAuth("payment-retrieval")]
        public async Task<IActionResult> Get(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _paymentRepository.Get(new Guid(id));

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (PaymentRepositoryException paymentRepositoryException)
            {
                _logger.LogError(paymentRepositoryException.ToString());

                return BadRequest(new PaymentRetrievalFailedResponse
                {
                    Success = false,
                    Message = $"An error has occurred when getting the payment record. Details: {paymentRepositoryException.Message}"
                });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.ToString());

                return BadRequest(new PaymentRetrievalFailedResponse
                {
                    Success = false,
                    Message = $"An unhandled error has occurred. Details: {exception.Message}"
                });
            }
        }
    }
}
