using System.Globalization;
using System.Text;
using TrueFit.Utilities.CurrencyTransformations;
using TrueFit.Utilities.DataTransformations;
using TrueFit.Utilities.Models;

namespace TrueFit.Services.CashRegisterChangeFile
{
    public class CashRegisterChangeFileLogic
    {
        #region ProcessCashRegisterChangeFile

        /// <summary>
        /// Process the file to determine the change amount.
        /// </summary>
        /// <param name="model">Model holding various parameters</param>
        /// <returns></returns>
        public string ProcessCashRegisterChangeFile(ProcessCashRegisterChangeFileModel model)
        {
            // Copy file to location where we can read. 
            if (Directory.Exists(model.UploadFileLocation) == false)
            {
                Directory.CreateDirectory(model.UploadFileLocation);
            }

            var fullSavePathAndFile = Path.Combine(model.UploadFileLocation, model.UploadFileName);
            using (var fileStream = new FileStream(fullSavePathAndFile, FileMode.Create))
            {
                model.UploadFile.CopyTo(fileStream);
            }

            var newFileContent = new StringBuilder();

            using (var reader = new StreamReader(fullSavePathAndFile))
            {
                var count = 1;
                while (reader.ReadLine() is { } line)
                {
                    // If the line has no value, then add a line break
                    if (line.Trim().Length == 0)
                    {
                        newFileContent.Append(Environment.NewLine);
                        continue;
                    }
                    
                    // Determine what the customer's change is. Returns a message, usually if there is an error, and the change amount.
                    var (errorMessage, change) = DetermineChangeAmount(line);

                    // If there is an error, return 
                    if (errorMessage.Length > 0)
                    {
                        newFileContent.Append(errorMessage);
                        continue;
                    }

                    // Determine denominations returned.
                    var decimalToCurrencyLogic = this.GetDecimalToCurrencyLogic(model.RandomDivisor, count);
                    var currencyValueInfoModel = decimalToCurrencyLogic.ConvertToCurrencyMessage(change, model.CurrencySettings);
                   
                    // Check change for issues. Explicitly states 'No Change' so user understands there is no
                    // error if nothing is returned.
                    var returnMessage = currencyValueInfoModel.CurrencyMessage;
                    if (currencyValueInfoModel.OriginalValue <= 0)
                    {
                        newFileContent.Append("No Change");
                        if (currencyValueInfoModel.OriginalValue < 0)
                        {
                            newFileContent.Append($" - Value paid is less than cost. {line}");
                        }
                    }
                    
                    newFileContent.Append(returnMessage + Environment.NewLine);

                    count += 1;
                }
            }

            return newFileContent.ToString();
        }

       #endregion

        #region GetDecimalToCurrencyLogic
      
        private IDecimalToCurrency GetDecimalToCurrencyLogic(int randomDivisor, int count)
        {
            if (randomDivisor> 0 && count % randomDivisor == 0)
            {
                return new DecimalToCurrencyRandomLogic();
            }
            return new DecimalToCurrencyLogic();
        }

        #endregion

        #region ProcessLineItem

        /// <summary>
        /// Determine the change for each line item in file.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private static (string errorMessage, decimal change) DetermineChangeAmount(string line)
        {
            var errorMessage = new StringBuilder();

            // If there is no comma to split by, then indicate a problem with the data.
            // We may want to set the delimiter value on the front end and pass through the method
            if (line.Contains(",") == false)
            {
                errorMessage.Append($"Invalid Format: {line} {Environment.NewLine}");
                return (errorMessage.ToString(), 0);
            }

            var items = line.Split(",");

            // Check to make sure there are only two values.
            if (items.Length != 2)
            {
                errorMessage.Append($"Invalid Format: {line} - Too many items {Environment.NewLine}");
                return (errorMessage.ToString(), 0);
            }

            var cost = StringDataTransformations.ConvertStringToDecimalNullable(items[0]);
            var paid = StringDataTransformations.ConvertStringToDecimalNullable(items[1]);

            if (cost == null || paid == null)
            {
                errorMessage.Append($"Invalid Format: {line}");
                if (cost == null)
                {
                    errorMessage.Append($" - Cost is not a decimal {items[0]}");
                }

                if (paid == null)
                {
                    errorMessage.Append($" - Amount paid is not a decimal {items[1]}");
                }
                errorMessage.Append(Environment.NewLine);
                return (errorMessage.ToString(), 0);
            }

            // Actual change amount calculation
            var change =  paid - cost;

            return (errorMessage.ToString(), (decimal)change);
        }

        #endregion

    }
}
