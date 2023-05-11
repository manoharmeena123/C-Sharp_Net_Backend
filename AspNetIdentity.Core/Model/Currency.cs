namespace AspNetIdentity.WebApi.Model
{
    public class Currency : DefaultFields
    {
        public int CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string ISOCurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string TransactionCurrencyId { get; set; }
    }
}