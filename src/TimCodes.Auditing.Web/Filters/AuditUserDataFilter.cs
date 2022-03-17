using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TimCodes.Auditing.Web.Filters;

/// <summary>
/// Filter that adds in the user subject claim to the audit scope
/// </summary>
public class AuditUserDataFilter : IActionFilter
{
    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is ControllerBase apiController)
        {
            IAuditScope? auditScope = apiController.GetCurrentAuditScope();
            var subject = context.HttpContext.User.FindFirst("sub")?.Value ??
                context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(subject))
            {
                auditScope.SetCustomField(Fields.SubjectId, subject);
            }
        }
    }
}
