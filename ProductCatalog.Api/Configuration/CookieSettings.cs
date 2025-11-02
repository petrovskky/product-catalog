namespace ProductCatalog.Api.Configuration;

public class CookieSettings
{
    public string Name { get; set; } = "auth-cookies";
    public bool HttpOnly { get; set; } = true;
    public bool Secure { get; set; } = false;
    public string SameSite { get; set; } = "None";
    public string Path { get; set; } = "/";
    public int ExpiresHours { get; set; } = 1;
    
    public SameSiteMode GetSameSiteMode() =>
        SameSite?.ToLower() switch
        {
            "none" => SameSiteMode.None,
            "lax" => SameSiteMode.Lax,
            _ => SameSiteMode.Strict
        };
}