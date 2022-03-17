using Microsoft.AspNetCore.Mvc;

namespace TimCodes.Auditing.Web;

public static class MvcOptionsExtensions
{
    /// <summary>
    /// Adds automatic auditing to actions. Be sure to add app.UseApiAuditing() too.
    /// By default we log all actions, including headers, and body.
    /// </summary>
    /// <param name="mvc"></param>
    /// <param name="serviceCollection">DI container</param>
    /// <param name="configure"></param>
    /// <param name="customFilterConfig"></param>
    public static void AddApiAuditing(this MvcOptions mvc,
        IServiceCollection serviceCollection,
        Action<ApiAuditOptions>? configure = null,
        Action<IAuditApiGlobalActionsSelector>? customFilterConfig = null)
    {
        if (customFilterConfig is not null)
        {
            mvc.AddAuditFilter(customFilterConfig);
        }
        else
        {
            mvc.AddAuditFilter(config => config
                .LogAllActions()
                .WithEventType("{verb}.{controller}.{action}")
                .IncludeHeaders()
                .IncludeRequestBody()
                .IncludeResponseHeaders()
                .IncludeResponseBody());
        }

        mvc.Filters.Add<AuditUserDataFilter>();
        mvc.Filters.Add<AuditCustomRedactFilter>();

        serviceCollection.AddSingleton<IRedactModeProvider, ActionFilterRedactModeProvider>();

        serviceCollection.AddAuditing(options =>
        {
            var apiOptions = new ApiAuditOptions(options.Services);

            configure?.Invoke(apiOptions);

            if (apiOptions.UseAuthorizationHeaderRedactor) options.AddRedactor<AuthorizationAuditRedactor>();
            if (apiOptions.UseCookiesRedactor) options.AddRedactor<CookiesAuditRedactor>();
            if (apiOptions.UseSensitveDataRedactor) options.AddRedactor<SensitiveDataAuditRedactor>();
            if (apiOptions.UseIpAddressStrictRedactor) options.AddRedactor<IpAddressStrictAuditRedactor>();
            if (apiOptions.UseRequestResponseBodyStrictRedactor) options.AddRedactor<RequestResponseBodyStrictAuditRedactor>();

            AuditConfiguration.SensitiveFields.AddRange(apiOptions.SensitiveFields);
        });
    }
}
