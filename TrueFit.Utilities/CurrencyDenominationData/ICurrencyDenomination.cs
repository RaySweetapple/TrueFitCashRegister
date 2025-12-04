
namespace TrueFit.Utilities.CurrencyDenominationData
{
    public interface ICurrencyDenomination
    {
        List<CurrencyDenominationModel> CurrencyModels { get; set; }

        void SetCurrency();
    }
}
