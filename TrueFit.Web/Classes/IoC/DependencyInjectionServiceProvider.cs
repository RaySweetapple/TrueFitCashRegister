using Scrutor;

namespace TrueFit.Web.Classes.IoC
{
    public static class DependencyInjectionServiceProvider
    {
        public static IServiceCollection GetDependencyInjectionServiceProvider(this IServiceCollection services)
        {
            // TrueFit.Services
            services.Scan(scan => scan.FromApplicationDependencies()
                              .AddClasses(c => c.InNamespaces("TrueFit.Services")
                                              .Where(w => w.FullName != null && w.FullName.Contains("Logic")), true)
                              .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                              .AsSelf()
                              .WithTransientLifetime()
                        );
           
            // TrueFit.Utilities
            services.Scan(scan => scan.FromApplicationDependencies()
                              .AddClasses(c => c.InNamespaces("TrueFit.Utilities")
                                              .Where(w => w.FullName != null && w.FullName.Contains("Logic")), true)
                              .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                              .AsSelf()
                              .WithTransientLifetime()
            );

            return services;
        }
    }
}
