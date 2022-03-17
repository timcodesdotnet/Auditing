using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace TimCodes.Auditing.Web;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Enable the re-reading of request body for auditing purposes
    /// </summary>
    /// <param name="app"></param>
    public static void UseApiAuditing(this IApplicationBuilder app) => 
        app.Use(async (context, next) =>
        {
            context.Request.EnableBuffering();
            await next();
        });
}
