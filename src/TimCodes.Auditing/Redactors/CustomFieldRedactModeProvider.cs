namespace TimCodes.Auditing.Redactors;

/// <summary>
/// Gets the redact mode from a custom field named RedactMode
/// </summary>
public class CustomFieldRedactModeProvider : IRedactModeProvider
{
    public int Priority { get; set; } = int.MaxValue;

    public RedactMode GetRedactMode(IAuditScope scope) =>
        scope.Event.CustomFields.ContainsKey("RedactMode")
            ? (RedactMode)Enum.Parse(typeof(RedactMode), (string)scope.Event.CustomFields["RedactMode"])
            : AuditConfiguration.DefaultRedactMode;
}
