using Audit.Core;
using TimCodes.Auditing.Redactors;

namespace TimCodes.Auditing.Web.UnitTests.Redactors;

public class CustomRedactor : IAuditRedactor
{
    public bool Redact(IAuditScope scope, RedactMode mode)
    {
        scope.Event.CustomFields.Add("Custom", true);
        return true;
    }
}
