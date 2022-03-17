namespace TimCodes.Auditing.Web.Redactors;

/// <summary>
/// Redacts the authorization header from an audit scope
/// </summary>
public class AuthorizationAuditRedactor : IAuditRedactor
{
    public bool Redact(IAuditScope scope, RedactMode mode)
    {
        if (mode == RedactMode.Loose) return false;

        if (scope.Event is AuditEventWebApi apiEvent)
        {
            if (apiEvent.Action?.Headers?.ContainsKey("Authorization") == true)
            {
                if (mode == RedactMode.Strict)
                {
                    apiEvent.Action.Headers["Authorization"] = Redacting.RedactToken;
                }
                else
                {
                    var auth = apiEvent.Action.Headers["Authorization"].Split(' ');
                    apiEvent.Action.Headers["Authorization"] = $"{auth[0]} {Redacting.RedactToken}";
                }

                return true;
            }
        }

        return false;
    }
}
