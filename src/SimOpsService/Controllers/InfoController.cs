using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PrabalGhosh.Utilities;
using Refit;
using SimOps.Models.Authentication;
using Swashbuckle.AspNetCore.Annotations;
using SimOpsService.Models;
using SimOpsService.Interfaces;

namespace SimOpsService.Controllers
{
    [Route("")]
    public class InfoController : BaseController
    {
        private IUserService _userService;
        private AuthenticationOptions _authOptions;
        private IAuthManager _authManager;

        public InfoController(IUserService userService, 
            AuthenticationOptions authOptions,
            IAuthManager authManager)
        {
            _userService = userService;
            _authOptions = authOptions;
            _authManager = authManager;
        }
        
        [HttpGet(Name = nameof(GetInfo))]
        [Produces("application/json")]
        public async Task<IActionResult> GetInfo()
        {
            return Ok(new
            {
                Message = "SimOps Web Service 1.0",
                CurrentMinimumLogLevel = Program.LogLevelSwitch.MinimumLevel,
                AvailableAt = await GetMyExternalIP()
            });
        }

        private async Task<string> GetMyExternalIP()
        {
            var client = RestService.For<IExternalIp>("http://ipv4.icanhazip.com/");
            var myIp = await client.GetMyExternalIp();
            return myIp.Trim();
        }


        [HttpPost("loglevel", Name = nameof(ChangeLogLevel)), Microsoft.AspNetCore.Authorization.Authorize("role:admin")]
        public async Task<IActionResult> ChangeLogLevel([FromBody] LogLevelRequest logLevelRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Unknown log level");
            }

            await Task.Run(() => { Program.LogLevelSwitch.MinimumLevel = logLevelRequest.LogLevel; });
            return Ok();
        }

        [HttpPost("login", Name = nameof(Login)), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request!");
            //handle scenario where login is called but auth is set to Auth0
            if (_authOptions.Method.ToLower().Equals("auth0"))
                return BadRequest("Invalid call! Use Auth0 for authentication...");
            User currentUser = null;
            //handle special case of sysadmin
            if (request.Username.ToLower().Equals(_authOptions.InternalAuth.SystemAdministration.Username.ToLower()))
            {
                //system administrator is attempting to login...
                if (request.Password.GetHash().Equals(_authOptions.InternalAuth.SystemAdministration.Password))
                {
                    //password matches!
                    //allow login...
                    currentUser = new User()
                    {
                        Created    = DateTime.UtcNow,
                        Email = _authOptions.InternalAuth.SystemAdministration.Email,
                        Role = "role:admin",
                        FirstName = "System",
                        LastLogin = DateTime.UtcNow,
                        LastName = "Administrator",
                        Username = _authOptions.InternalAuth.SystemAdministration.Username
                    };
                }
                else
                {
                    return BadRequest("Invalid username/password");
                }
            }
            else
            {
                //regular user is trying to login
                //look for the user in the database...
                var found = await _userService.FindByUsername(request.Username);
                if (found.IsNull())
                    return BadRequest("Invalid username/password");
                //does the password match?
                var testPass = request.Password.GetHash(found.Salt);
                if (testPass.Equals(found.Password))
                {
                    //password matches
                    currentUser = found.Clone();
                }
                else
                {
                    return BadRequest("Invalid username/password");
                }
            }

            if (currentUser.IsNotNull())
            {
                //issue claims and generate jwt token
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, currentUser.Username),
                    new Claim(ClaimTypes.Email, currentUser.Email),
                    new Claim(ClaimTypes.GivenName, currentUser.FirstName),
                    new Claim(ClaimTypes.Surname, currentUser.LastName),
                    new Claim(ClaimTypes.Role, currentUser.Role),
                    new Claim("scope", $"openid profile email {currentUser.Role}"),
                    new Claim("permissions", $"[\"{currentUser.Role}\"]")
                };
                var token = await _authManager.GenerateToken(currentUser.Username, claims);
                if (token.IsNull())
                    return BadRequest("Invalid username/password");
                return Ok(token);
            }
            return BadRequest("Invalid username/password");
        }
    }
}
