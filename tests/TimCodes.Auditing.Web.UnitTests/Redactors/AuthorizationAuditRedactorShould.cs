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
public class AuthorizationAuditRedactorShould
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    public AuthorizationAuditRedactorShould()
    {
        // Arrange
        _server = new TestServer(new WebHostBuilder()
           .UseStartup<TestStartup>());
        _client = _server.CreateClient();
    }


    [TestMethod]
    public async Task RedactAuthHeader()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/action");
        httpRequestMessage.Headers.Add("Authorization", "Bearer test");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        var e = StaticDataProvider.AuditEvents.Last();

        var action = e.GetWebApiAuditAction();
        Assert.AreEqual($"Bearer {Redacting.RedactToken}", action.Headers["Authorization"]);
    }
}
