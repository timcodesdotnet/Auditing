namespace TimCodes.Auditing.Redactors;

/// <summary>
/// Defines how strict the redactors configured for your app should be
/// </summary>
public enum RedactMode
{
    /// <summary>
    /// No redacting will take place
    /// </summary>
    None,

    /// <summary>
    /// Only custom redacting will take place
    /// </summary>
    Loose,

    /// <summary>
    /// Sensitive information will be redacted
    /// </summary>
    Medium,

    /// <summary>
    /// Minimal data is kept
    /// </summary>
    Strict
}
