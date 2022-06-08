using System.Threading.Tasks;
using SimOps.Models.Authentication;

namespace SimOpsService.Interfaces;

public interface IUserService
{
    Task<User> FindByUsername(string username);
}