using SimOps.Models.Authentication;

namespace SimOpsService.Models;

public class Token
{
    public string AccessToken { get; set; }
    public RefreshToken RefreshToken { get; set; }
}