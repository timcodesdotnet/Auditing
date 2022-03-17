using Audit.WebApi;
using TimCodes.Auditing.Constants;
using TimCodes.Auditing.Web.UnitTests.DataProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TimCodes.Auditing.Web.UnitTests.Redactors;

[TestClass]
public class IpAddressAuditRedactorShould
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    public IpAddressAuditRedactorShould()
    {
        // Arrange
        _server = new TestServer(new WebHostBuilder()
           .UseStartup<TestStartup>());
        _client = _server.CreateClient();
    }


    [TestMethod]
    public async Task RedactIpAddressInRequestHeaderWhenStrict()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/strict");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        var e = StaticDataProvider.AuditEvents.Last();

        var action = e.GetWebApiAuditAction();
        Assert.AreEqual(Redacting.RedactToken, action.IpAddress);
    }
}
