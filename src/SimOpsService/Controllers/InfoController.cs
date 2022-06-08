using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using SimOpsService.Models;
using Refit;
using SimOpsService.Interfaces;

namespace SimOpsService.Controllers
{
    [Route("")]
    public class InfoController : BaseController
    {
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


        [HttpPost("loglevel", Name = nameof(ChangeLogLevel))]
        public async Task<IActionResult> ChangeLogLevel([FromBody] LogLevelRequest logLevelRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Unknown log level");
            }

            await Task.Run(() => { Program.LogLevelSwitch.MinimumLevel = logLevelRequest.LogLevel; });
            return Ok();
        }
    }
}
