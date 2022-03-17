namespace TimCodes.Auditing.DataProviders;

/// <summary>
/// Logs audits to the standard ASP.NET logger
/// </summary>
public class AspNetLoggerAuditDataProvider : AuditDataProvider
{
    private readonly ILogger _logger;

    public AspNetLoggerAuditDataProvider(ILogger<AspNetLoggerAuditDataProvider> logger)
    {
        _logger = logger;
    }

    public override object? InsertEvent(AuditEvent auditEvent)
    {
        _logger.LogInformation("Audit logged: {audit}", auditEvent.ToJson());
        return null;
    }
}
