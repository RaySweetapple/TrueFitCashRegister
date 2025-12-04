using Microsoft.AspNetCore.Mvc;
using TrueFit.Services.CashRegisterChangeFile;
using TrueFit.Utilities.CurrencyDenominationData;
using TrueFit.Web.Classes;
using TrueFit.Web.Models;

namespace TrueFit.Web.Controllers
{
    public class HomeController(CashRegisterChangeFileLogic cashRegisterChangeFileLogic) : Controller
    {
        #region Error
        
        public IActionResult Error()
        {
            return View();
        }

        #endregion

        #region Index

        [HttpGet]
        public IActionResult Index()
        {
            var model = new HomeViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(HomeViewModel model)
        {
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            // Validate Extensions. Typically set as an application setting.
            var extension = Path.GetExtension(model.UploadFile.FileName);
            if (extension.ToLower() != ".txt")
            {
                ModelState.AddModelError(nameof(model.UploadFile),"The import file is not one of the following extensions: .txt.");
            }
            
            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            // Process the data. Note that we pass the currency and random divisor settings into the method. 
            // If we want to give the user the ability to change in the form, it can be easily supported.
            var processModel = new ProcessCashRegisterChangeFileModel(model.UploadFile, ApplicationSettings.UploadFileLocation, ApplicationSettings.DefaultRandomDivisor, new UnitedStatesCurrencyDenomination());
            var newFileContent = cashRegisterChangeFileLogic.ProcessCashRegisterChangeFile(processModel);
            
            // Set the file data
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(newFileContent);

            return File(fileBytes, "text/plain", processModel.UploadFileName);
          
        }

        #endregion
    }
}
