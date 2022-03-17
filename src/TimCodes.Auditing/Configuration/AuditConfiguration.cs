namespace TimCodes.Auditing.Configuration;

/// <summary>
/// Static app-wide configuration for auditing
/// </summary>
public static class AuditConfiguration
{
    private static readonly List<Type> _defaultRedactors = new();

    /// <summary>
    /// Defines a list of sensitive fields to redact
    /// </summary>
    public static List<string> SensitiveFields { get; set; } = new();

    /// <summary>
    /// When not specified, this will be the default <see cref="RedactMode"/> used
    /// </summary>
    public static RedactMode DefaultRedactMode { get; internal set; }

    /// <summary>
    /// Defines a list of data providers that will be used by the <see cref="MultiplexingAuditDataProvider"/>
    /// </summary>
    public static List<MultiplexedAuditDataProvider> DataProviders { get; internal set; } = new List<MultiplexedAuditDataProvider>();

    internal static IList<Type> GetDefaultRedactors() => _defaultRedactors;

    public static void ClearDefaultRedactors() => _defaultRedactors.Clear();

    public static void RemoveDefaultRedactor<T>() => _defaultRedactors.Remove(typeof(T));

    /// <summary>
    /// Add a redactor to the list of default redactors
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="order"></param>
    public static void AddDefaultRedactor<T>(int? order = null)
        where T : IAuditRedactor
    {
        if (order.HasValue)
        {
            _defaultRedactors.Insert(order.Value, typeof(T));
        }
        else
        {
            _defaultRedactors.Add(typeof(T));
        }
    }
}
