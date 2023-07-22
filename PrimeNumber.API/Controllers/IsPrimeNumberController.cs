using API.Helpers;
using Client.DataStore;
using Microsoft.AspNetCore.Mvc;

namespace PrimeNumber.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IsPrimeNumberController : ControllerBase
    {
        private readonly IClientService _dataservice;

        public IsPrimeNumberController(ClientDataContext dataContext, IConfiguration configuration)
        {
            // When fails to load set default to 10
            if (!int.TryParse(configuration["rateLimit"], out int rateLimit) || rateLimit < 0)
                rateLimit = 10;

            _dataservice = new ClientService(rateLimit, dataContext);
        }

        [HttpPost("{number}/{token}")]
        public IActionResult Post(string number, string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token) || token.Length > 100)
                {
                    return BadRequest("Invalid token.");
                }

                if (string.IsNullOrWhiteSpace(number) ||
                    number.Length > 10 ||
                    !int.TryParse(number, out int inputInt))
                {
                    return BadRequest("Invalid number.");
                }

                if (_dataservice.HasRateLimitExceeded(token, out double remainingMinutes))
                    return StatusCode(
                        StatusCodes.Status403Forbidden,
                        new
                        {
                            HasError = true,
                            ErrorMessage = $"You have exceeded you call limits, Remaining time untill restriction removed is {remainingMinutes} minute(s)"
                        });

                return Ok(new
                {
                    HasError = false,
                    Number = inputInt,
                    IsPrime = MathHelper.IsPrimeNumber(inputInt)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new
                    {
                        HasError = true,
                        ErrorMessage = ex.Message
                    });
            }
        }
    }
}