namespace TrueFit.Utilities.CurrencyTransformations
{
    public class DecimalToCurrencyRandomLogic : AbstractDecimalToCurrency
    {
        /// <summary>
        /// Calculate the currency denominations to return
        /// </summary>
        /// <param name="totalCents">The total cents to be paid</param>
        /// <param name="currencyValue">The currency value</param>
        /// <returns></returns>
        public override int CalculateDenominations(int totalCents, int currencyValue)
        {
            var result = totalCents / currencyValue;

            // Randomize the results between 0 and the result, if not 1.
            if (currencyValue != 1 )
            {
                var rnd = new Random();
                result = rnd.Next(0, result);
            }
            return result;
        }

    }
}
