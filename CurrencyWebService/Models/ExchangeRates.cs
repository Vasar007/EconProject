using System;
using System.Collections.Generic;

namespace EconProject.CurrencyWebService.Models
{
    public class ExchangeRates
    {
        public string Base { get; set; }

        public Dictionary<string, decimal> Rates { get; set; }

        public DateTime Date { get; set; }


        public ExchangeRates()
        {
        }
    }
}
