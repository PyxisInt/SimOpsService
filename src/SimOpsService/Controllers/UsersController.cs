using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimOpsService.Controllers;

[Route("/api/v1/[controller]")]
[Authorize("role:admin")]
public class UsersController : BaseController
{
    
}