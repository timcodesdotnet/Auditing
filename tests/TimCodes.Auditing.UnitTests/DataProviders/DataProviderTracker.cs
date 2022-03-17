using System;

namespace TimCodes.Auditing.UnitTests.DataProviders;

public class DataProviderTracker
{
    public static Type LastCalled { get; set; }
}
