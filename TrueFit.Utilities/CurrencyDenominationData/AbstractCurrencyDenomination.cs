
namespace TrueFit.Utilities.CurrencyDenominationData
{
    public abstract class AbstractCurrencyDenomination : ICurrencyDenomination
    {
        public List<CurrencyDenominationModel> CurrencyModels { get; set; } = [];

        public virtual void SetCurrency()
        {
           
        }
    }
}
