
using NLog.Web;
using TrueFit.Utilities.ExceptionManagement;

namespace TrueFit.Web
{
    public class Program
    {
        private static readonly NLog.ILogger Log = NLog.LogManager.GetLogger("ErrorLogger");

        public static void Main(string[] args)
        {
            // Catch all for exceptions 
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {               
                Log.Error(EnvironmentExceptionFormatter.Format(ex, null, "Error logged in the program.cs file main method."));
              
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                // Check for captive dependency issues with asp.net core default IoC container
                .UseDefaultServiceProvider((env, c) =>
                                           {
                                               if (env.HostingEnvironment.IsDevelopment())
                                               {
                                                   c.ValidateScopes = true;
                                               }
                                           })
                .ConfigureWebHostDefaults(webBuilder =>
                                          {
                                              webBuilder.CaptureStartupErrors(false);
                                              webBuilder.UseStartup<Startup>();
                                          })
                .UseNLog(new NLogAspNetCoreOptions
                {
                    RemoveLoggerFactoryFilter = false  // Respect the log level filters in appsettings
                }) 

                ;

       

        #region UnhandledExceptionTrapper

        /// <summary>
        /// Traps all unhandled exceptions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            Log.Error(EnvironmentExceptionFormatter.Format(ex, null, "Error logged in the program.cs file in UnhandledExceptionTrapper method."));
          
        }
        #endregion
    }
}
