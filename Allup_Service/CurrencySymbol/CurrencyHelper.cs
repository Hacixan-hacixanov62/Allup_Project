namespace Allup_Service.CurrencySymbol
{
    public static class CurrencyHelper
    {
        public static string GetSymbol(string currency) =>
        currency switch
        {
            "USD" => "$",
            "EUR" => "€",
            "AZN" => "₼",
            _ => currency
        };
    }
}
