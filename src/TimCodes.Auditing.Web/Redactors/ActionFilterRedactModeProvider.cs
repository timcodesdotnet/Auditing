namespace TimCodes.Auditing.Web.Redactors;

/// <summary>
/// Pulls the RedactMode for the current audit scope from the action executing context
/// </summary>
public class ActionFilterRedactModeProvider : IRedactModeProvider
{
    public int Priority { get; set; } = int.MaxValue - 1;

    public RedactMode GetRedactMode(IAuditScope scope)
    {
        var apiScope = scope.Event as AuditEventWebApi;
        ActionExecutingContext? context = apiScope?.Action?.GetActionExecutingContext();
        if (context != null)
        {
            if (context.Filters.LastOrDefault(q => q is AuditRedactModeAttribute) is AuditRedactModeAttribute redactAttribute)
            {
                return redactAttribute.RedactMode;
            }
        }

        return AuditConfiguration.DefaultRedactMode;
    }
}
