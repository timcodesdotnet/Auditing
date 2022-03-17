namespace TimCodes.Auditing.Redactors;

/// <summary>
/// Does redacting on an audit before it is saved
/// </summary>
public interface IAuditRedactor
{
    /// <summary>
    /// Run the redactor against the given scope in the given mode
    /// </summary>
    /// <param name="scope"></param>
    /// <param name="mode"></param>
    /// <returns>True if the redactor ran, otherwise false (eg if mode set to loose or scope not matching prereqs)</returns>
    bool Redact(IAuditScope scope, RedactMode mode);
}
