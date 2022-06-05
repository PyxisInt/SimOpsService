using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimOpsService.Attributes;

namespace SimOpsService.Controllers
{
    [ApiController]
    [OperationId]
    [TransactionHeader]
    [UserContext]
    public class BaseController : ControllerBase
    {
        public string RequestId { get; set; }
        public  string CurrentToken { get; set; }
        public string CurrentUser { get; set; }
    }
}
