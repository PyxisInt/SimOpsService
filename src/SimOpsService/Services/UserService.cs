using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimOps.Models.Authentication;
using SimOpsService.Interfaces;
using SimOpsService.Repository;

namespace SimOpsService.Services;

public class UserService : IUserService
{
    private SimOpsContext _db;

    public UserService(SimOpsContext db)
    {
        _db = db;
    }
    
    public async Task<User> FindByUsername(string username)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
    }
}