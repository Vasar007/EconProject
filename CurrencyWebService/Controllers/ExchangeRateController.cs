using System;
using System.Threading.Tasks;
using EconProject.CurrencyWebService.Domain;
using Microsoft.AspNetCore.Mvc;

namespace EconProject.CurrencyWebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangeRateController : ControllerBase
    {
        private readonly IExchangeRateProcessorAsync _exchangeRateProcessor;


        public ExchangeRateController(IExchangeRateProcessorAsync exchangeRateProcessor)
        {
            _exchangeRateProcessor = exchangeRateProcessor;
        }

        [HttpGet]
        public async Task<ActionResult<decimal>> GetExchangeRate(string @base, string target)
        {
            try
            {
                decimal exchangeRate;
                if (string.IsNullOrEmpty(@base) || string.IsNullOrEmpty(target))
                {
                    exchangeRate = await _exchangeRateProcessor.GetExchangeRateAsync("USD", "EUR");
                }
                else
                {
                    exchangeRate = await _exchangeRateProcessor.GetExchangeRateAsync(@base, target);
                }
                return exchangeRate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Exception occured: {ex}");
            }
            return BadRequest(new[] { @base, target });
        }
    }
}
