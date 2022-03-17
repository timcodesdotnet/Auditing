using TimCodes.Auditing.Redactors;
using TimCodes.Auditing.Web.UnitTests.DataProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TimCodes.Auditing.Web.UnitTests.Redactors;

[TestClass]
public class RedactorLogShould
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    public RedactorLogShould()
    {
        // Arrange
        _server = new TestServer(new WebHostBuilder()
           .UseStartup<TestStartup>());
        _client = _server.CreateClient();
    }


    [TestMethod]
    public async Task FillRedactedBy()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/strict");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        var e = StaticDataProvider.AuditEvents.Last();
        var log = e.CustomFields["RedactedBy"] as RedactLog;

        Assert.AreEqual(RedactMode.Strict.ToString(), log.Mode);
        Assert.IsTrue(log.Redactors.Contains("IpAddressStrict"));
    }

    [TestMethod]
    public async Task ContainCustomRedactors()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/custom");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        var e = StaticDataProvider.AuditEvents.Last();
        var log = e.CustomFields["RedactedBy"] as RedactLog;

        Assert.AreEqual(RedactMode.Medium.ToString(), log.Mode);
        Assert.IsTrue(log.Redactors.Contains("Custom"));
    }
}
