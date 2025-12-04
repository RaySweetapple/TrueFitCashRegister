using System.Net;
using Microsoft.AspNetCore.Http.Features;
using TrueFit.Web.Classes;
using TrueFit.Web.Classes.IoC;

namespace TrueFit.Web
{
    public class Startup
    {

        #region Constructor

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            this.Configuration = builder.Build();
            this.Environment = env;
        }

        #endregion

        #region Public Properties

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        #endregion

        #region ConfigureServices

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // https://stackoverflow.com/questions/63362022/collection-bound-to-model-exceeded-mvcoptions-maxmodelbindingcollectionsize-1
            // Set the max number of form values that can be model bound.
            // Note there are additional options https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.mvcoptions.allowemptyinputinbodymodelbinding?view=aspnetcore-6.0
            services.Configure<FormOptions>(options =>
                                            {
                                                options.ValueCountLimit = 5000;
                                            });

            services.AddControllersWithViews(options =>
                                             {
                                                 // If SuppressImplicitRequiredAttributeForNonNullableReferenceTypes not set to true,
                                                 // it will assume nullable reference types are required for form validation.
                                                 options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                                             }).AddJsonOptions(jsonOptions =>
                                                               {
                                                                   //Prevent the built-in JsonResult serializer from camel-casing property names
                                                                   jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
                                                               }).AddSessionStateTempDataProvider(); //use Session for TempData Dictionary rather than a cookie

          
           

            
            //Initialize GlobalVariables.cs(must be done before everything that depends on it)
            ApplicationSettings.Initialize(this.Configuration);


            // Dependency Injection
            services.GetDependencyInjectionServiceProvider();
           
        }

        #endregion

        #region Configure

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                var options = new DeveloperExceptionPageOptions
                {
                    SourceCodeLineCount = 5
                };
                app.UseDeveloperExceptionPage(options);
              
            }
            else
            {
                

                // HTTP Strict Transport Security Protocol (HSTS)
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            // Supposedly forces the site to redirect to https
            // Also note you need to add "https_port": 443, to the appSettings.json file. Note you can set this in different ways other than using the json file.
            app.UseHttpsRedirection();

            // Add headers
            app.Use( async (context, next) =>
                    {
                        context.Response.OnStarting(() =>
                            {
                                context.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
                                context.Response.Headers["X-Content-Type-Options"] = "nosniff";
                                context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
                                
                                return Task.FromResult(0);
                            });
                        await next();
                    }
            );

            // 404-Page Redirects
            app.Use(async (context, next) =>
                    {
                        await next();
                      
                        if (context.Response.StatusCode is 404 or 403)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            context.Request.Path = "/home/error";
                            await next();
                        }

                    });

           
            app.UseRouting();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        #endregion

    }
}
