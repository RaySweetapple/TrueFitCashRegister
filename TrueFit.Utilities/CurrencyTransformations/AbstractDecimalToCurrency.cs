using System.Text;
using TrueFit.Utilities.CurrencyDenominationData;
using TrueFit.Utilities.Models;

namespace TrueFit.Utilities.CurrencyTransformations
{
    public abstract class AbstractDecimalToCurrency : IDecimalToCurrency
    {
        /// <summary>
        /// Calculate the denominations to return.
        /// </summary>
        /// <param name="totalCents"></param>
        /// <param name="currencyValue"></param>
        /// <returns></returns>
        public virtual int CalculateDenominations(int totalCents, int currencyValue)
        {
            var result = totalCents / currencyValue;
            return result;
        }

        /// <summary>
        /// Convert the currency value to the appropriate denominations of the currency.
        /// </summary>
        /// <param name="value">Remaining value</param>
        /// <param name="currencyInfo">Contains settings for changing to denominations</param>
        /// <returns></returns>
        public CurrencyValueInfoModel ConvertToCurrencyMessage(decimal value, ICurrencyDenomination currencyInfo)
        {
            var returnModel = new CurrencyValueInfoModel { OriginalValue = value, CurrencyInfo = currencyInfo };
            // This assumes there is no CurrencyMessage when there is no value or the value is negative
            if (value <= 0)
            {
                return returnModel;
            }
            
            // Set the running total of remaining cents
            var totalCents = (int)Math.Round(value * 100);

            // Make sure the list is in order of max value to least.
            var currencyModels = currencyInfo.CurrencyModels.OrderByDescending(x => x.CurrencyValue);
            
            var returnString = new StringBuilder();
            foreach (var model in currencyModels)
            {
                var currencyValue = (int)(model.CurrencyValue * 100);
             
                // Determine, if any, the number of denominations to return
                var result = this.CalculateDenominations(totalCents, currencyValue);
                if (result == 0)
                {
                    continue;
                }

                // Subtract from the running total cents
                totalCents -= (currencyValue * result);

                if (returnString.Length > 0)
                {
                    returnString.Append(", ");
                }
                returnString.Append($"{result} ");
                returnString.Append(result == 1 ? model.CurrencyName : model.CurrencyNamePlural);
                
            }
            returnModel.CurrencyMessage = returnString.ToString().Trim();

            return returnModel;
        }
    }
}
