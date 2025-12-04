namespace TrueFit.Utilities.CurrencyDenominationData
{
    public class UnitedStatesCurrencyDenomination : AbstractCurrencyDenomination
    {

        public UnitedStatesCurrencyDenomination()
        {
           this.SetCurrency();
        }

        public sealed override void SetCurrency()
        {
            this.CurrencyModels.Add( new CurrencyDenominationModel
            {
                CurrencyName = "Dollar",
                CurrencyNamePlural = "Dollars",
                CurrencyValue = 1
            });

            this.CurrencyModels.Add( new CurrencyDenominationModel
            {
                CurrencyName = "Quarter",
                CurrencyNamePlural = "Quarters",
                CurrencyValue = 0.25m
            });

            this.CurrencyModels.Add( new CurrencyDenominationModel
            {
                CurrencyName = "Dime",
                CurrencyNamePlural = "Dimes",
                CurrencyValue = 0.10m
            });

            this.CurrencyModels.Add( new CurrencyDenominationModel
            {
                CurrencyName = "Nickel",
                CurrencyNamePlural = "Nickels",
                CurrencyValue = 0.05m
            });

            this.CurrencyModels.Add( new CurrencyDenominationModel
            {
                CurrencyName = "Penny",
                CurrencyNamePlural = "Pennies",
                CurrencyValue = 0.01m
            });

        }
    }
}
