using TrueFit.Utilities.DataTransformations;

namespace TrueFit.Web.Classes
{
    internal static class ApplicationSettings
    {
        private static IConfigurationSection? appSettings;
        public static void Initialize(IConfiguration configuration)
        {
            appSettings = configuration.GetSection("AppSettings");
        }

        #region DefaultRandomDivisor

        /// <summary>
        /// Default random divisor setting.
        /// </summary>
        public static int DefaultRandomDivisor => StringDataTransformations.ConvertStringToInt(appSettings?["DefaultRandomDivisor"]) ;

        #endregion
        
        #region UploadFileLocation

        /// <summary>
        /// Upload file location. Ideally the location would be outside of this project.
        /// </summary>
        public static string UploadFileLocation => appSettings?["UploadFileLocation"] ?? string.Empty ;

        #endregion
    }
}
