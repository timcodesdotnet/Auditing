namespace TimCodes.Auditing.DataProviders;

/// <summary>
/// A wrapper for a data provider to be used inside the <see cref="MultiplexingAuditDataProvider"/>
/// </summary>
public class MultiplexedAuditDataProvider
{
    public MultiplexedAuditDataProvider(Type type, int order)
    {
        if (!typeof(AuditDataProvider).IsAssignableFrom(type))
        {
            throw new ArgumentException($"Must inherit from {nameof(AuditDataProvider)}", nameof(type));
        }

        DataProviderType = type;
        Order = order;
    }

    /// <summary>
    /// The AuditDataProvider type to use
    /// </summary>
    public Type DataProviderType { get; }

    /// <summary>
    /// Order that the data provider will sit in the multiplexer
    /// </summary>
    public int Order { get; }

    /// <summary>
    /// Predicate to be checked before sending an AuditEvent to the data provider
    /// </summary>
    public Func<AuditEvent, bool> Predicate { get; set; } = scope => true;
}

public class MultiplexedAuditDataProvider<T> : MultiplexedAuditDataProvider
{
    public MultiplexedAuditDataProvider(int order) : base(typeof(T), order)
    {

    }
}
