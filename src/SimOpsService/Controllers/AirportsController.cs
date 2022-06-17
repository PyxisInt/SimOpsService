using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimOpsService.Controllers;

[Route("/api/v1/[controller]")]
[AllowAnonymous]
public class AirportsController : BaseController
{
    
}