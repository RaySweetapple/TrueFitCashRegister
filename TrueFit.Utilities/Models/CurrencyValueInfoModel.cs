using TrueFit.Utilities.CurrencyDenominationData;

namespace TrueFit.Utilities.Models
{
    public class CurrencyValueInfoModel
    {
        public string? CurrencyMessage { get; set; }
        public ICurrencyDenomination? CurrencyInfo { get; set; }
        public decimal OriginalValue { get; set; }
    }
}
