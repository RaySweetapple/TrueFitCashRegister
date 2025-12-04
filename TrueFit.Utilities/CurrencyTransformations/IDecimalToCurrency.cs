using TrueFit.Utilities.CurrencyDenominationData;
using TrueFit.Utilities.Models;

namespace TrueFit.Utilities.CurrencyTransformations
{
    public interface IDecimalToCurrency
    {
        int CalculateDenominations(int totalCents, int currencyValue);

        CurrencyValueInfoModel ConvertToCurrencyMessage(decimal value, ICurrencyDenomination currencyInfo);
    }
}
