namespace TimCodes.Auditing.Web.Configuration;

public class ApiAuditOptions : AuditOptions
{
    public ApiAuditOptions(IServiceCollection services) : base(services)
    {
    }

    public bool UseAuthorizationHeaderRedactor { get; set; } = true;
    public bool UseCookiesRedactor { get; set; } = true;
    public bool UseIpAddressStrictRedactor { get; set; } = true;
    public bool UseSensitveDataRedactor { get; set; } = true;
    public bool UseRequestResponseBodyStrictRedactor { get; set; } = true;
    public List<string> SensitiveFields { get; set; } = new List<string>
        {
            "password",
            "cardNumber",
            "access_token",
            "refresh_token",
            "cvv",
            "cv2",
            "csc"
        };

}
