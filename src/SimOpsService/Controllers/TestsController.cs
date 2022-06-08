using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrabalGhosh.Utilities.Attributes;

namespace SimOpsService.Controllers;

[Route("api/v1/[controller]")]
public class TestsController : BaseController
{
    [HttpGet(Name = nameof(GetAuthenticatedInfo))]
    [Authorize("role:admin")]
    public async Task<IActionResult> GetAuthenticatedInfo()
    {
        return Ok(new
        {
            Name = "Information",
            Value = "Authentication worked!"
        });
    }
}