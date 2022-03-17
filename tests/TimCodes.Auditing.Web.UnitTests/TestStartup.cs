using TimCodes.Auditing.Web.UnitTests.DataProvider;
using TimCodes.Auditing.Web.UnitTests.Redactors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TimCodes.Auditing.Web.UnitTests;

public class TestStartup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<CustomRedactor>();
        services.AddControllers(mvc => mvc.AddApiAuditing(services, options =>
            {
                options.AddRedactor<RedactModeTestRedactor>();
                options.AddMultiplexedDataProvider<StaticDataProvider>(1);
            }));
    }

    public static void Configure(IApplicationBuilder app)
    {
        app.UseApiAuditing();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
