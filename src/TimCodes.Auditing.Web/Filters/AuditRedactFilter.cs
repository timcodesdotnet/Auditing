using Microsoft.AspNetCore.Mvc;

namespace TimCodes.Auditing.Web.Filters;

/// <summary>
/// Filter that adds in custom redactors per action
/// </summary>
public class AuditCustomRedactFilter : IActionFilter
{
    private readonly IServiceProvider _provider;

    public AuditCustomRedactFilter(IServiceProvider provider)
    {
        _provider = provider;
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is ControllerBase apiController)
        {
            IAuditScope? auditScope = apiController.GetCurrentAuditScope();
            IEnumerable<AuditRedactAttribute> redactAttributes = context.Filters.Where(q => q is AuditRedactAttribute).Cast<AuditRedactAttribute>();
            IRedactModeProvider? redactModeProvider = _provider.GetServices<IRedactModeProvider>().OrderBy(q => q.Priority).FirstOrDefault();

            foreach (AuditRedactAttribute a in redactAttributes)
            {
                var redactor = _provider.GetRequiredService(a.RedactorType) as IAuditRedactor;

                if (redactor is not null && !auditScope.HasRedacted(redactor))
                {
                    RedactMode mode = redactModeProvider?.GetRedactMode(auditScope) ?? AuditConfiguration.DefaultRedactMode;

                    if (redactor.Redact(auditScope, mode))
                    {
                        auditScope.LogRedactor(redactor, mode);
                    }
                }
            }
        }
    }
}
