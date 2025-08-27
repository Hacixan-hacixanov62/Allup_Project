namespace Allup_Service.Service.IService
{
    public interface ICurrencyService
    {
        decimal ConvertCurrencyAsync(decimal amount, string fromCurrency, string toCurrency);
    }
}
