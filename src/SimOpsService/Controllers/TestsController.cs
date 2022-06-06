using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimOpsService.Controllers;

[Route("api/v1/[controller]")]
[Authorize]
public class TestsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAuthenticatedInfo()
    {
        return Ok(new
        {
            Name = "Information",
            Value = "Authentication worked!"
        });
    }
}