using TimCodes.Auditing.Web.Attributes;
using TimCodes.Auditing.Web.UnitTests.Redactors;
using Microsoft.AspNetCore.Mvc;

namespace TimCodes.Auditing.Web.UnitTests.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    [HttpPost]
    [Route("action")]
    public IActionResult Action()
    {
        Response.Cookies.Append("test", "secretpassword");

        return Ok(new
        {
            Password = "secretpassword"
        });
    }

    [HttpGet]
    [HttpPost]
    [Route("strict")]
    [AuditRedactMode(Auditing.Redactors.RedactMode.Strict)]
    public IActionResult Strict()
    {
        return new JsonResult(new
        {
            Password = "secretpassword"
        });
    }

    [HttpGet]
    [HttpPost]
    [Route("custom")]
    [AuditRedact(typeof(CustomRedactor))]
    public IActionResult Custom()
    {
        return new JsonResult(new
        {
            Password = "secretpassword"
        });
    }
}
