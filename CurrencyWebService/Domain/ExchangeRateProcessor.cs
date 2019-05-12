using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RestSharp;
using EconProject.CurrencyWebService.Models;

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

        private static decimal ExtractTargetExchangeRate(IRestResponse response, string baseCurrency,
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

            var exchangeRate = ratesToken.Value<decimal>(targetCurrency);
            return exchangeRate;
        }

        private static ExchangeRates ExtractExchangeRate(IRestResponse response,
            string baseCurrency, string[] targetCurrencies)
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
                !IsEqualWithInvariantCulture(parsedJson["base"].Value<string>(), baseCurrency) ||
                targetCurrencies.Any(targetCurrency => ratesToken[targetCurrency] is null))
            {
                throw new InvalidOperationException(
                    "Response does not correspond to the requested data."
                );
            }

            var exchangeRates = parsedJson.ToObject<ExchangeRates>();
            return exchangeRates;
        }

        #region IExchangeRateProcessor Implementation

        public async Task<decimal> GetTargetExchangeRateAsync(string baseCurrency,
            string targetCurrency)
        {
            IRestResponse response = await SendGetTargetExchangeRateQuery(
                baseCurrency, targetCurrency
            );
            decimal exchangeRate = ExtractTargetExchangeRate(response, baseCurrency,
                                                             targetCurrency);
            return exchangeRate;
        }

        public async Task<ExchangeRates> GetExchangeRateAsync(string baseCurrency,
            string[] targetCurrencies)
        {
            IRestResponse response = await SendGetExchangeRateQuery(
                baseCurrency, targetCurrencies
            );
            ExchangeRates exchangeRate = ExtractExchangeRate(response, baseCurrency,
                                                             targetCurrencies);
            return exchangeRate;
        }

        #endregion

        private async Task<IRestResponse> SendGetTargetExchangeRateQuery(string baseCurrency,
            string targetCurrency)
        {
            var request = new RestRequest(Method.GET);

            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("base", baseCurrency, ParameterType.QueryString);
            request.AddParameter("symbols", targetCurrency, ParameterType.QueryString);

            IRestResponse response = await _restClient.ExecuteTaskAsync(request);
            return response;
        }

        private async Task<IRestResponse> SendGetExchangeRateQuery(string baseCurrency,
            string[] targetCurrencies)
        {
            var request = new RestRequest(Method.GET);

            request.AddParameter("undefined", "{}", ParameterType.RequestBody);
            request.AddParameter("base", baseCurrency, ParameterType.QueryString);
            request.AddParameter("symbols", string.Join(',', targetCurrencies),
                                 ParameterType.QueryString);

            IRestResponse response = await _restClient.ExecuteTaskAsync(request);
            return response;
        }
    }
}
