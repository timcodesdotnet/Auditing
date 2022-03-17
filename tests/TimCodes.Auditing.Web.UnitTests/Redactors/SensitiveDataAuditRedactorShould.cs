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
public class SensitiveDataAuditRedactorShould
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    public SensitiveDataAuditRedactorShould()
    {
        // Arrange
        _server = new TestServer(new WebHostBuilder()
           .UseStartup<TestStartup>());
        _client = _server.CreateClient();
    }


    [TestMethod]
    public async Task RedactPasswordInRequestBody()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/action")
        {
            Content = new StringContent("password=secretpassword")
        };
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();

        var e = StaticDataProvider.AuditEvents.Last();

        var action = e.GetWebApiAuditAction();
        Assert.IsFalse(((string)action.RequestBody.Value).Contains("secretpassword"));
    }

    [TestMethod]
    public async Task RedactPasswordInResponseBody()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/action");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();

        var e = StaticDataProvider.AuditEvents.Last();

        var action = e.GetWebApiAuditAction();
        Assert.IsFalse((action.ResponseBody.Value.ToString()).Contains("secretpassword"));
    }

    [TestMethod]
    public async Task RedactPasswordInQuerystring()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "/test/action?password=secretpassword");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();

        var e = StaticDataProvider.AuditEvents.Last();

        var action = e.GetWebApiAuditAction();
        Assert.IsFalse((action.RequestUrl).Contains("secretpassword"));
    }
}
