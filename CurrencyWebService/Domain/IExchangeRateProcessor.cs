using System.Threading.Tasks;
using EconProject.CurrencyWebService.Models;

namespace EconProject.CurrencyWebService.Domain
{
    public interface IExchangeRateProcessorAsync
    {
        Task<decimal> GetTargetExchangeRateAsync(string baseCurrency, string targetCurrency);

        Task<ExchangeRates> GetExchangeRateAsync(string baseCurrency, string[] targetCurrencies);
    }
}
