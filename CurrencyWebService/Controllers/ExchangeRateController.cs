using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EconProject.CurrencyWebService.Domain;
using EconProject.CurrencyWebService.Models;

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

        [HttpGet("{base}")]
        public async Task<ActionResult<decimal>> GetTargetExchangeRate(string @base, string target)
        {
            try
            {
                decimal exchangeRate;
                if (string.IsNullOrEmpty(@base) || string.IsNullOrEmpty(target))
                {
                    exchangeRate =
                        await _exchangeRateProcessor.GetTargetExchangeRateAsync("USD", "EUR");
                }
                else
                {
                    exchangeRate =
                        await _exchangeRateProcessor.GetTargetExchangeRateAsync(@base, target);
                }
                return exchangeRate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Exception occured: {ex}");
            }
            return BadRequest(new[] { @base, target });
        }

        [HttpGet]
        public async Task<ActionResult<ExchangeRates>> GetExchangeRate(string @base,
            [FromQuery(Name = "target")] string[] target)
        {
            try
            {
                ExchangeRates exchangeRate;
                if (string.IsNullOrEmpty(@base) ||
                    (!(target is null) && target.All(t => string.IsNullOrEmpty(t))))
                {
                    exchangeRate =
                        await _exchangeRateProcessor.GetExchangeRateAsync("USD", new[] { "EUR" });
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
            return BadRequest(new[] { @base, string.Join(',', target) });
        }
    }
}
