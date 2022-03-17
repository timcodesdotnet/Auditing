namespace TimCodes.Auditing.Web.Attributes;

/// <summary>
/// Defines a custom redactor for this action
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
public class AuditRedactAttribute : ActionFilterAttribute
{
    public AuditRedactAttribute(Type redactorType)
    {
        if (!typeof(IAuditRedactor).IsAssignableFrom(redactorType))
            throw new ArgumentException($"Type does not implement {nameof(IAuditRedactor)}", nameof(redactorType));

        RedactorType = redactorType;
    }

    /// <summary>
    /// Defines the redact mode for this action
    /// </summary>
    public Type RedactorType { get; set; }
}
