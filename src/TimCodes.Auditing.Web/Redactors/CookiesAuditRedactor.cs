namespace TimCodes.Auditing.Web.Redactors;

/// <summary>
/// Redacts the cookies in request and response headers from an audit scope
/// </summary>
public class CookiesAuditRedactor : IAuditRedactor
{
    public bool Redact(IAuditScope scope, RedactMode mode)
    {
        if (mode == RedactMode.Loose) return false;

        if (scope.Event is AuditEventWebApi apiEvent)
        {
            var hasRan = false;
            //Cookie format is like "key=value; key2=value"
            if (apiEvent.Action?.Headers?.ContainsKey("Cookie") == true)
            {
                if (mode == RedactMode.Strict)
                {
                    apiEvent.Action.Headers["Cookie"] = Redacting.RedactToken;
                }
                else
                {
                    var cookies = apiEvent.Action.Headers["Cookie"].Split(';', StringSplitOptions.RemoveEmptyEntries);
                    apiEvent.Action.Headers["Cookie"] = string.Join(";", cookies.Select(q =>
                    {
                        var split = q.Split('=');
                        return $"{split[0].TrimStart(' ')}={Redacting.RedactToken}";
                    }));
                }

                hasRan = true;
            }

            //Set-Cookie format is like "test=cookie; path=/, testing=cookie; path=/; HttpOnly"
            if (apiEvent.Action?.ResponseHeaders?.ContainsKey("Set-Cookie") == true)
            {
                if (mode == RedactMode.Strict)
                {
                    apiEvent.Action.ResponseHeaders["Set-Cookie"] = Redacting.RedactToken;
                }
                else
                {
                    var setCookies = apiEvent.Action.ResponseHeaders["Set-Cookie"].Split(',', StringSplitOptions.RemoveEmptyEntries);
                    apiEvent.Action.ResponseHeaders["Set-Cookie"] = string.Join(", ", setCookies.Select(q =>
                    {
                        var parts = q.Split(';', StringSplitOptions.RemoveEmptyEntries);
                        var firstPart = $"{parts[0].Split('=')[0].TrimStart(' ')}={Redacting.RedactToken};";
                        return firstPart + string.Join(";", parts.Skip(1));
                    }));
                }

                hasRan = true;
            }

            return hasRan;
        }

        return false;
    }
}
