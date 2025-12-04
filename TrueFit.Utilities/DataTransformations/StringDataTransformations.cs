namespace TrueFit.Utilities.DataTransformations
{
    public static class StringDataTransformations
    {
        #region ConvertStringToDecimal

        /// <summary>
        /// Convert a string to a decimal value type. If invalid returns 0.
        /// </summary>
        /// <param name="value">String to be converted</param>
        /// <param name="decimalPlaces">Number of decimal places for rounding</param>
        /// <returns></returns>
        public static decimal ConvertStringToDecimal(string? value, int? decimalPlaces = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            if (decimal.TryParse(value, out var decimalValue))
            {
                if (decimalPlaces != null)
                {
                    return Math.Round(decimalValue, (int)decimalPlaces, MidpointRounding.ToEven);
                }

                return decimalValue;
            }
            return 0;
        }

        #endregion

        #region ConvertStringToDecimalNullable
       
        /// <summary>
        /// Convert string to decimal. If the value in invalid, returns null
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static decimal? ConvertStringToDecimalNullable(string? value, int? decimalPlaces = null)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            if (decimal.TryParse(value, out var decimalValue))
            {
                if (decimalPlaces != null)
                {
                    return Math.Round(decimalValue, (int)decimalPlaces, MidpointRounding.ToEven);
                }

                return decimalValue;
            }
            return null;
        }

        #endregion

        #region ConvertStringToInt
        
        /// <summary>
        /// Convert string to int, if invalid returns 0.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ConvertStringToInt(string? value)
        {
            var result = int.TryParse(value, out var i);
            return result ? i : 0;
        }
       
        #endregion
    }
}
