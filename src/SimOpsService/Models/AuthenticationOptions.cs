namespace SimOpsService.Models;

public class AuthenticationOptions
{
    public string Method { get; set; }
    public Auth0 Auth0 { get; set; }
    public InternalAuth InternalAuth { get; set; }
}

public class Auth0
{
    public string Authority { get; set; }
    public string ApiIdentifier { get; set; }
}

public class InternalAuth
{
    public JwtConfig JwtConfig { get; set; }
    public SystemAdministration SystemAdministration { get; set; }
}

public class JwtConfig
{
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpiration { get; set; }
    public int RefreshTokenExpiration { get; set; }
}

public class SystemAdministration
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}