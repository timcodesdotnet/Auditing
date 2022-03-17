using TimCodes.Auditing.Extensions;

namespace TimCodes.Auditing;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add services and default redactors required for auditing using Audit.NET. 
    /// The <see cref="MultiplexingAuditDataProvider"/> is set up with no providers as default. 
    /// Use <see cref="AddMultiplexedDataProvider"/> before this call to register some.
    /// </summary>
    /// <param name="serviceCollection"></param>
    /// <param name="configure"></param>
    public static void AddAuditing(this IServiceCollection serviceCollection,
        Action<AuditOptions>? configure = null)
    {
        var options = new AuditOptions(serviceCollection);

        serviceCollection.AddSingleton<MultiplexingAuditDataProvider>();
        serviceCollection.AddSingleton<IRedactModeProvider, CustomFieldRedactModeProvider>();

        configure?.Invoke(options);

        AuditConfiguration.DefaultRedactMode = options.DefaultRedactMode;

        options.AddRedactors();

        ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
        MultiplexingAuditDataProvider dataProvider = serviceProvider.GetRequiredService<MultiplexingAuditDataProvider>();

        Audit.Core.ConfigurationApi.IConfigurator config = Audit.Core.Configuration.Setup();
        config.UseCustomProvider(dataProvider);
        options.CustomizeInternalConfig?.Invoke(config);
    }

    /// <summary>
    /// Register a singleton in the service collection for the data provider and add it to the list of multiplexed providers in <see cref="AuditConfiguration.DataProviders"/>
    /// </summary>
    /// <typeparam name="T">Type of the data provider</typeparam>
    /// <param name="options"></param>
    /// <param name="order">Order to place the data provider in the list</param>
    /// <param name="predicate">Predicate to check if the data provider should be used for the given event</param>
    public static AuditOptions AddMultiplexedDataProvider<T>(this AuditOptions options, int order = 1, Func<AuditEvent, bool>? predicate = null)
        where T : AuditDataProvider
    {
        options.Services.AddSingleton<T>();
        var m = new MultiplexedAuditDataProvider(typeof(T), order)
        {
            Predicate = predicate ?? (q => true)
        };
        AuditConfiguration.DataProviders.Add(m);
        AuditConfiguration.DataProviders = AuditConfiguration.DataProviders.OrderBy(q => q.Order).ToList();
        return options;
    }

    /// <summary>
    /// Adds a redactor to the default collection and registers it with DI
    /// </summary>
    /// <param name="options"></param>
    public static AuditOptions AddRedactor<T>(this AuditOptions options)
        where T : IAuditRedactor
    {
        options.Services.AddSingleton(typeof(T));
        AuditConfiguration.AddDefaultRedactor<T>();
        return options;
    }

    private static AuditOptions AddRedactors(this AuditOptions options)
    {
        ServiceProvider serviceProvider = options.Services.BuildServiceProvider();
        IServiceProvider sp = serviceProvider.GetRequiredService<IServiceProvider>();

        Audit.Core.Configuration.AddCustomAction(Audit.Core.ActionType.OnEventSaving, auditScope =>
        {
            IRedactModeProvider? redactModeProvider = sp.GetServices<IRedactModeProvider>().OrderBy(q => q.Priority).FirstOrDefault();

            RedactMode mode = redactModeProvider?.GetRedactMode(auditScope) ?? AuditConfiguration.DefaultRedactMode;
            if (mode == RedactMode.None) return;

            IEnumerable<IAuditRedactor> redactors = AuditConfiguration.GetDefaultRedactors().Select(type => (IAuditRedactor)sp.GetRequiredService(type));

            foreach (IAuditRedactor redactor in redactors)
            {
                if (!auditScope.HasRedacted(redactor))
                {
                    if (redactor.Redact(auditScope, mode))
                    {
                        auditScope.LogRedactor(redactor, mode);
                    }
                }
            }
        });
        return options;
    }
}
