namespace TimCodes.Auditing.Extensions;

public static class AuditScopeExtensions
{
    private const string RedactedBy = "RedactedBy";

    private static string GetRedactorNameForLog(IAuditRedactor redactor) =>
        redactor.GetType().Name
            .Replace("AuditRedactor", string.Empty)
            .Replace("Redactor", string.Empty);

    /// <summary>
    /// Log the redactor in the audit event
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="redactor"></param>
    /// <returns></returns>
    public static void LogRedactor(this IAuditScope scope, IAuditRedactor redactor, RedactMode mode)
    {
        RedactLog? log;
        if (!scope.Event.CustomFields.ContainsKey(RedactedBy))
        {
            scope.SetCustomField(RedactedBy, new RedactLog()
            {
                Mode = mode.ToString()
            });
        }

        log = scope.Event.CustomFields[RedactedBy] as RedactLog;
        if (log is not null)
        {
            log.Redactors.Add(GetRedactorNameForLog(redactor));
        }
    }

    /// <summary>
    /// Check if the redactor has been run already against the scope
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="redactor"></param>
    /// <returns></returns>
    public static bool HasRedacted(this IAuditScope scope, IAuditRedactor redactor) => 
        scope.Event.CustomFields.ContainsKey(RedactedBy) &&
               scope.Event.CustomFields[RedactedBy] is RedactLog log &&
               log.Redactors.Contains(GetRedactorNameForLog(redactor));
}
