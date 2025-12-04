using TrueFit.Utilities.CurrencyDenominationData;
using TrueFit.Utilities.CurrencyTransformations;

namespace TrueFit.UnitTests.Utilities.CurrencyTransformationTests
{
    [TestClass]
    public class DecimalToCurrencyTest
    {
        #region DecimalToCurrency

        #region DecimalToCurrencyUsdValueNegative

        [DataTestMethod]
        public void DecimalToCurrencyUsdValueNegative()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(-1.87m, currencyInfo);

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(result.CurrencyMessage));
        }

        #endregion

        #region DecimalToCurrencyNoValue

        [DataTestMethod]
        public void DecimalToCurrencyNoValue()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(0,currencyInfo);

            // Assert
            Assert.IsTrue(string.IsNullOrEmpty(result.CurrencyMessage));
        }

        #endregion

        #region DecimalToCurrencyUsdValue1

        [DataTestMethod]
        public void DecimalToCurrencyUsdValue1()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(1,currencyInfo);

            // Assert
            Assert.IsTrue(result.CurrencyMessage == "1 Dollar");
        }

        #endregion

        #region DecimalToCurrencyUsdValue99

        [DataTestMethod]
        public void DecimalToCurrencyUsdValue99()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(0.99m,currencyInfo);

            // Assert
            Assert.IsTrue(result.CurrencyMessage == "3 Quarters, 2 Dimes, 4 Pennies");
        }

        #endregion

        #region DecimalToCurrencyUsdValue55_25

        [DataTestMethod]
        public void DecimalToCurrencyUsdValue55_25()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(55.25m,currencyInfo);

            // Assert
            Assert.IsTrue(result.CurrencyMessage == "55 Dollars, 1 Quarter");
        }

        #endregion

        #region DecimalToCurrencyUsdValue_25

        [DataTestMethod]
        public void DecimalToCurrencyUsdValue_25()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(.25m,currencyInfo);

            // Assert
            Assert.IsTrue(result.CurrencyMessage == "1 Quarter");
        }

        #endregion

        #region DecimalToCurrencyUsdValue_15

        [DataTestMethod]
        public void DecimalToCurrencyUsdValue_15()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(.15m,currencyInfo);

            // Assert
            Assert.IsTrue(result.CurrencyMessage == "1 Dime, 1 Nickel");
        }

        #endregion
        
        #region DecimalToCurrencyUsdValue_10

        [DataTestMethod]
        public void DecimalToCurrencyUsdValue_10()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(.10m,currencyInfo);

            // Assert
            Assert.IsTrue(result.CurrencyMessage == "1 Dime");
        }

        #endregion

        #region DecimalToCurrencyUsdValue_05

        [DataTestMethod]
        public void DecimalToCurrencyUsdValue_05()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(.05m,currencyInfo);

            // Assert
            Assert.IsTrue(result.CurrencyMessage == "1 Nickel");
        }

        #endregion

        #region DecimalToCurrencyUsdValue_01

        [DataTestMethod]
        public void DecimalToCurrencyUsdValue_01()
        {
            var decimalToCurrency = new DecimalToCurrencyLogic();
            var currencyInfo = new UnitedStatesCurrencyDenomination();

            var result = decimalToCurrency.ConvertToCurrencyMessage(.03m,currencyInfo);

            // Assert
            Assert.IsTrue(result.CurrencyMessage == "3 Pennies");
        }

        #endregion

        #endregion

      
    }
}
