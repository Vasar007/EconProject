using System.Threading.Tasks;

namespace EconProject.CurrencyWebService.Domain
{
    public interface IExchangeRateProcessorAsync
    {
        Task<double> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
    }
}
