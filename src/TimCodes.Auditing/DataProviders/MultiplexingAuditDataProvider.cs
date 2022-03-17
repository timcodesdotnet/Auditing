namespace TimCodes.Auditing.DataProviders;

/// <summary>
/// Logs audits to multiple data providers
/// </summary>
public class MultiplexingAuditDataProvider : AuditDataProvider
{
    private readonly IServiceProvider _serviceProvider;

    public MultiplexingAuditDataProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private AuditDataProvider? GetAuditDataProvider(MultiplexedAuditDataProvider provider, AuditEvent auditEvent)
    {
        if (provider.Predicate(auditEvent))
        {
            var service = _serviceProvider.GetRequiredService(provider.DataProviderType) as AuditDataProvider;
            return service;
        }

        return null;
    }

    public override object? InsertEvent(AuditEvent auditEvent)
    {
        foreach (MultiplexedAuditDataProvider provider in AuditConfiguration.DataProviders)
        {
            AuditDataProvider? service = GetAuditDataProvider(provider, auditEvent);
            service?.InsertEvent(auditEvent);
        }

        return null;
    }

    public override async Task<object?> InsertEventAsync(AuditEvent auditEvent)
    {
        foreach (MultiplexedAuditDataProvider provider in AuditConfiguration.DataProviders)
        {
            AuditDataProvider? service = GetAuditDataProvider(provider, auditEvent);
            if (service is not null)
            {
                await service.InsertEventAsync(auditEvent);
            }
        }

        return null;
    }

    public override void ReplaceEvent(object eventId, AuditEvent auditEvent)
    {
        foreach (MultiplexedAuditDataProvider provider in AuditConfiguration.DataProviders)
        {
            AuditDataProvider? service = GetAuditDataProvider(provider, auditEvent);
            service?.ReplaceEvent(eventId, auditEvent);
        }
    }

    public override async Task ReplaceEventAsync(object eventId, AuditEvent auditEvent)
    {
        foreach (MultiplexedAuditDataProvider provider in AuditConfiguration.DataProviders)
        {
            AuditDataProvider? service = GetAuditDataProvider(provider, auditEvent);
            if (service is not null)
            {
                await service.ReplaceEventAsync(eventId, auditEvent);
            }
        }
    }
}
