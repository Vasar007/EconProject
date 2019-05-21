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
                if (string.IsNullOrEmpty(@base))
                {
                    throw new ArgumentException("Base currency couldn't be empty.", nameof(@base));
                }
                if (string.IsNullOrEmpty(target))
                {
                    throw new ArgumentException("Target currency couldn't be empty.",
                                                nameof(target));
                }


                decimal exchangeRate =
                        await _exchangeRateProcessor.GetTargetExchangeRateAsync(@base, target);
                return exchangeRate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Exception occured: {ex}");
                return BadRequest(new[] { ex.Message, @base, target });
            }
        }

        [HttpGet]
        public async Task<ActionResult<ExchangeRates>> GetExchangeRate(string @base,
            [FromQuery(Name = "target")] string[] target)
        {
            try
            {
                if (string.IsNullOrEmpty(@base))
                {
                    throw new ArgumentException("Base currency couldn't be empty.", nameof(@base));
                }
                if ((!(target is null) && target.All(t => string.IsNullOrEmpty(t))))
                {
                    throw new ArgumentException("Target currencies contains invalid value.",
                                                nameof(target));
                }

                ExchangeRates exchangeRate =
                    await _exchangeRateProcessor.GetExchangeRateAsync(@base, target);
                return exchangeRate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($@"Exception occured: {ex}");
                return BadRequest(new[] { ex.Message, @base, string.Join(',', target) });
            }
        }
    }
}
