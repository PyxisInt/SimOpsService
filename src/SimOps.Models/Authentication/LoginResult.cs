namespace SimOps.Models.Authentication;

public class LoginResult
{
    public string Username { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}