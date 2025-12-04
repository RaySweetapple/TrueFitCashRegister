using Microsoft.AspNetCore.Http;
using TrueFit.Utilities.CurrencyDenominationData;


namespace TrueFit.Services.CashRegisterChangeFile
{
    public class ProcessCashRegisterChangeFileModel(IFormFile uploadFile, string uploadFileLocation, int randomDivisor, ICurrencyDenomination currencySettings)
    {
        public ICurrencyDenomination CurrencySettings { get; } = currencySettings;
        public int RandomDivisor { get; } = randomDivisor;
        public IFormFile UploadFile { get; } = uploadFile;
        public string UploadFileLocation { get; } = uploadFileLocation;
        public string UploadFileName
        {
            get
            {
                var fileName = Path.GetFileName(this.UploadFile.FileName);
                return DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;
            }
        }
    }
}
