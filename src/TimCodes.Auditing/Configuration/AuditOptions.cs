using Audit.Core.ConfigurationApi;

namespace TimCodes.Auditing.Configuration;

public class AuditOptions
{
    public AuditOptions(IServiceCollection services)
    {
        Services = services;
    }

    internal IServiceCollection Services { get; set; }

    /// <summary>
    /// Delegate to allow you to customise the internal Audit.NET config
    /// </summary>
    public Action<IConfigurator>? CustomizeInternalConfig { get; set; }

    /// <summary>
    /// Defaults to Medium
    /// </summary>
    public RedactMode DefaultRedactMode { get; set; } = RedactMode.Medium;
}
