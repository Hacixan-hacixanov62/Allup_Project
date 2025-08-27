using Allup_Service.Service.IService;

namespace Allup_Service.Service
{
    public class CurrencyService : ICurrencyService
    {
        private readonly Dictionary<string, decimal> _rates = new Dictionary<string, decimal>
        {
        {"USD", 1m},
        {"EUR", 0.91m},
        {"AZN", 1.70m}
        };

        public decimal ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency)
        {
            if (!_rates.ContainsKey(fromCurrency) || !_rates.ContainsKey(toCurrency))
                throw new ArgumentException("Düzgün valyuta deyil");

            decimal inUsd = amount / _rates[fromCurrency];
            return inUsd * _rates[toCurrency];
        }
    }
}
  