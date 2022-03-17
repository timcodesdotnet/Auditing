namespace TimCodes.Auditing.Web.Redactors;

/// <summary>
/// Redacts the body of request and response headers from an audit scope
/// </summary>
public class RequestResponseBodyStrictAuditRedactor : IAuditRedactor
{
    public bool Redact(IAuditScope scope, RedactMode mode)
    {
        if (mode == RedactMode.Loose) return false;

        if (mode == RedactMode.Strict)
        {
            var apiEvent = scope?.Event as AuditEventWebApi;
            if (apiEvent?.Action != null)
            {
                apiEvent.Action.RequestBody = new BodyContent
                {
                    Type = Redacting.RedactToken
                };
                apiEvent.Action.ResponseBody = new BodyContent
                {
                    Type = Redacting.RedactToken
                };
                return true;
            }
        }

        return false;
    }
}
