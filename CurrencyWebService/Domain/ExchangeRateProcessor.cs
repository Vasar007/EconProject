using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace EconProject.CurrencyWebService.Domain
{
    public class ExchangeRateProcessor : IExchangeRateProcessorAsync
    {
        private readonly ServiceSettings _settings;

        private readonly RestClient _restClient;


        public ExchangeRateProcessor(IOptions<ServiceSettings> settingsOptions)
        {
            _settings = settingsOptions.Value;
            _restClient = new RestClient(_settings.ExchangeRateApiUrl);
        }

        private static bool IsEqualWithInvariantCulture(string str, string other)
        {
            return string.Compare(str, other, StringComparison.InvariantCulture) == 0;
        }

        private static JObject GetResponseResult(IRestResponse response)
        {
            JObject parsedJson = JObject.Parse(response.Content);
            return parsedJson;
        }

        private static double ExtractExchangeRate(IRestResponse response, string baseCurrency,
            string targetCurrency)
        {
            // Example of response: {"base":"USD","rates":{"EUR":0.8904719501},"date":"2019-05-10"}
            JObject parsedJson = GetResponseResult(response);
            JToken ratesToken = parsedJson["rates"];

            if (!(parsedJson["error"] is null) ||
                ratesToken is null)
            {
                throw new InvalidOperationException("Request failed.");
            }
            if (!ratesToken.HasValues ||
                !IsEqualWithInvariantCulture(parsedJson["base"].Value<string>(), baseCurrency))
            {
                throw new InvalidOperationException(
                    "Response does not correspond to the requested data."
                );
            }

            var exchangeRate = ratesToken.Value<double>(targetCurrency);
            return exchangeRate;
        }

        #region IExchangeRateProcessor Implementation

        public async Task<double> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
        {
            IRestResponse response = await SendGetExchangeRateQuery(baseCurrency, targetCurrency);
            double exchangeRate = ExtractExchangeRate(response, baseCurrency, targetCurrency);
            return exchangeRate;
        }

        #endregion

        private async Task<IRestResponse> SendGetExchangeRateQuery(string baseCurrency,
            string targetCurrency)
        {
            var request = new RestRequest(Method.GET);

            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("base", baseCurrency, ParameterType.QueryString);
            request.AddParameter("symbols", targetCurrency, ParameterType.QueryString);

            IRestResponse response = await _restClient.ExecuteTaskAsync(request);
            return response;
        }
    }
}
