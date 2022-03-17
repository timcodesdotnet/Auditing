using Audit.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TimCodes.Auditing.UnitTests.DataProviders;

public class StaticDataProvider : AuditDataProvider
{
    public static List<AuditEvent> AuditEvents = new List<AuditEvent>();

    public override void ReplaceEvent(object eventId, AuditEvent auditEvent)
    {
        DataProviderTracker.LastCalled = this.GetType();
        AuditEvents[(int)eventId] = auditEvent;
    }

    public override object InsertEvent(AuditEvent auditEvent)
    {
        DataProviderTracker.LastCalled = this.GetType();
        AuditEvents.Add(auditEvent);
        return AuditEvents.Count - 1;
    }

    public override Task<object> InsertEventAsync(AuditEvent auditEvent)
    {
        return Task.FromResult(InsertEvent(auditEvent));
    }
}
