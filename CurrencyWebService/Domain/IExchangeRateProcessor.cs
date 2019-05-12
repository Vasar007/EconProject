using System.Threading.Tasks;

namespace EconProject.CurrencyWebService.Domain
{
    public interface IExchangeRateProcessorAsync
    {
        Task<decimal> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
    }
}
