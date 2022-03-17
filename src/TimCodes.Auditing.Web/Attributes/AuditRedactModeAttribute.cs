namespace TimCodes.Auditing.Web.Attributes;

/// <summary>
/// Defines the redact mode for this action
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class AuditRedactModeAttribute : ActionFilterAttribute
{
    public AuditRedactModeAttribute(RedactMode redactMode)
    {
        RedactMode = redactMode;
    }

    /// <summary>
    /// Defines the redact mode for this action
    /// </summary>
    public RedactMode RedactMode { get; set; }
}
