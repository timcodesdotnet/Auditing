using Audit.WebApi;
using TimCodes.Auditing.Web.UnitTests.DataProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TimCodes.Auditing.Web.UnitTests.Redactors;

[TestClass]
public class CookiesAuditRedactorShould
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    public CookiesAuditRedactorShould()
    {
        // Arrange
        _server = new TestServer(new WebHostBuilder()
           .UseStartup<TestStartup>());
        _client = _server.CreateClient();
    }


    [TestMethod]
    public async Task RedactCookiesInRequestHeader()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/action");
        httpRequestMessage.Headers.Add("Cookie", "test=secretpassword");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        var e = StaticDataProvider.AuditEvents.Last();

        var action = e.GetWebApiAuditAction();
        Assert.IsFalse(action.Headers["Cookie"].Contains("secretpassword"));
    }

    [TestMethod]
    public async Task RedactCookiesInResponseHeader()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/action");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        var e = StaticDataProvider.AuditEvents.Last();

        var action = e.GetWebApiAuditAction();
        Assert.IsFalse(action.ResponseHeaders["Set-Cookie"].Contains("secretpassword"));
    }
}
