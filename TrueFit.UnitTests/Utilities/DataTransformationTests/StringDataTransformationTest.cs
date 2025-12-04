using TrueFit.Utilities.DataTransformations;

namespace TrueFit.UnitTests.Utilities.DataTransformationTests
{
    [TestClass]
    public class StringDataTransformationTest
    {
        #region ConvertStringToDecimal

        #region ConvertStringToDecimalNull

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("0")]
        [DataRow("0.0")]
        [DataRow("a")]
        [DataRow("000a")]
        public void ConvertStringToDecimalNull(string? value)
        {
          var result = StringDataTransformations.ConvertStringToDecimal(value);

          Assert.IsTrue(result == 0);
        }

        #endregion

        #region ConvertStringToDecimal1_00

        [DataTestMethod]
        [DataRow("1")]
        [DataRow("1.0")]
        public void ConvertStringToDecimal1_00(string? value)
        {
            var result = StringDataTransformations.ConvertStringToDecimal(value);

            Assert.IsTrue(result == 1);
        }

        #endregion

        #region ConvertStringToDecimal_0_03_2

        [DataTestMethod]
        [DataRow("0.02501")]
        [DataRow("0.029")]
        [DataRow("0.03")]
        [DataRow("0.030")]
        [DataRow("0.0301")]
        [DataRow("0.031")]
        [DataRow("0.034999")]
        [DataRow(".03")]
        public void ConvertStringToDecimal_0_03_2(string? value)
        {
            var result = StringDataTransformations.ConvertStringToDecimal(value, 2);

            Assert.IsTrue(result == 0.03m);
        }

        #endregion

        #region ConvertStringToDecimal_1_53_2

        [DataTestMethod]
        [DataRow("1.52501")]
        [DataRow("1.5251")]
        [DataRow("1.529")]
        [DataRow("1.53")]
        [DataRow("1.530")]
        [DataRow("1.5301")]
        [DataRow("1.531")]
        [DataRow("1.534999")]
        public void ConvertStringToDecimal_1_53_2(string? value)
        {
            var result = StringDataTransformations.ConvertStringToDecimal(value, 2);

            Assert.IsTrue(result == 1.53m);
        }

        #endregion

        #endregion
    }
}
