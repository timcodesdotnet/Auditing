namespace TimCodes.Auditing.Redactors;

/// <summary>
/// Retrieves the chosen RedactMode for the context of the audit scope
/// </summary>
public interface IRedactModeProvider
{
    int Priority { get; set; }
    RedactMode GetRedactMode(IAuditScope scope);
}
