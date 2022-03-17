using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Audit.Core;
using System.Linq;

namespace TimCodes.Auditing.UnitTests.DataProviders;

[TestClass]
public class MultiplexingAuditDataProviderShould
{
    public MultiplexingAuditDataProviderShould()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddAuditing(options =>
        {
            options.AddMultiplexedDataProvider<StaticDataProvider>(order: 2);
            options.AddMultiplexedDataProvider<SecondStaticDataProvider>(order: 1, predicate: q => q.EventType.StartsWith("Test"));
        });

        serviceCollection.BuildServiceProvider();
    }

    [TestMethod]
    public void AuditToMultipleProviders()
    {
        StaticDataProvider.AuditEvents.Clear();
        SecondStaticDataProvider.AuditEvents.Clear();
        var auditScope = AuditScope.Create("TestEvent", () => new { });
        auditScope.Save();

        Assert.IsTrue(StaticDataProvider.AuditEvents.Last().EventType == "TestEvent");
        Assert.IsTrue(SecondStaticDataProvider.AuditEvents.Last().EventType == "TestEvent");
    }

    [TestMethod]
    public void HonourPredicate()
    {
        StaticDataProvider.AuditEvents.Clear();
        SecondStaticDataProvider.AuditEvents.Clear();
        var auditScope = AuditScope.Create("MyEvent", () => new { });
        auditScope.Save();

        Assert.IsTrue(StaticDataProvider.AuditEvents.Last().EventType == "MyEvent");
        Assert.IsFalse(SecondStaticDataProvider.AuditEvents.Any());
    }

    [TestMethod]
    public void HonourOrder()
    {
        StaticDataProvider.AuditEvents.Clear();
        SecondStaticDataProvider.AuditEvents.Clear();
        var auditScope = AuditScope.Create("TestEvent", () => new { });
        auditScope.Save();

        Assert.IsTrue(DataProviderTracker.LastCalled == typeof(StaticDataProvider));
    }
}
