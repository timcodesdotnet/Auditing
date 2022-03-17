namespace TimCodes.Auditing.Web.Redactors;

/// <summary>
/// Redacts the IP address in request headers from an audit scope
/// </summary>
public class IpAddressStrictAuditRedactor : IAuditRedactor
{
    public bool Redact(IAuditScope scope, RedactMode mode)
    {
        if (mode == RedactMode.Loose) return false;
        if (mode == RedactMode.Strict)
        {
            var apiEvent = scope?.Event as AuditEventWebApi;
            if (apiEvent?.Action != null)
            {
                apiEvent.Action.IpAddress = Redacting.RedactToken;
                return true;
            }
        }

        return false;
    }
}
