using TimCodes.Auditing.Web.UnitTests.DataProvider;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TimCodes.Auditing.Web.UnitTests.Filters;

[TestClass]
public class AuditRedactFilterShould
{
    private readonly TestServer _server;
    private readonly HttpClient _client;

    public AuditRedactFilterShould()
    {
        // Arrange
        _server = new TestServer(new WebHostBuilder()
           .UseStartup<TestStartup>());
        _client = _server.CreateClient();
    }


    [TestMethod]
    public async Task UseCustomRedactor()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "/test/custom");
        var response = await _client.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();
        var e = StaticDataProvider.AuditEvents.Last();

        Assert.IsTrue((bool)e.CustomFields["Custom"]);
    }
}
