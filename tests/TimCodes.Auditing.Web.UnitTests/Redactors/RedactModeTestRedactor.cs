using Audit.Core;
using TimCodes.Auditing.Redactors;

namespace TimCodes.Auditing.Web.UnitTests.Redactors;

public class RedactModeTestRedactor : IAuditRedactor
{
    public bool Redact(IAuditScope scope, RedactMode mode)
    {
        if (scope.Event.CustomFields.ContainsKey("RedactMode"))
        {
            scope.Event.CustomFields["RedactMode"] = mode.ToString();
        }
        else
        {
            scope.Event.CustomFields.Add("RedactMode", mode.ToString());
        }

        return true;
    }
}
