using Microsoft.AspNetCore.Mvc;
using Refit;
using SimOps.Models.Authentication;

namespace SimOps.Sdk.ApiDefinition;

public interface ILoginService
{
    [Post("/login")]
    Task<IActionResult> LoginAsync([Body] LoginRequest request);
}