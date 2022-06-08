using System.Security.Claims;
using System.Threading.Tasks;
using SimOps.Models.Authentication;
using SimOpsService.Models;

namespace SimOpsService.Interfaces;

public interface IAuthManager
{
    Task<LoginResult> GenerateToken(string userName, Claim[] claims);
}