using Audit.Core;
using TimCodes.Auditing.Redactors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimCodes.Auditing.UnitTests.Redactors;

[TestClass]
public class CustomFieldRedactModelProviderShould
{
    [TestMethod]
    public void GetValueFromCustomField()
    {
        var provider = new CustomFieldRedactModeProvider();
        var scope = AuditScope.Create("Test", () => this, new { RedactMode = "Strict" });
        var redactMode = provider.GetRedactMode(scope);
        Assert.AreEqual(RedactMode.Strict, redactMode);
    }
}
