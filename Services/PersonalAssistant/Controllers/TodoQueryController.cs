using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace PersonalAssistant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonalAssistantController : ControllerBase
    {
        private readonly ILogger<PersonalAssistantController> _logger;

        public PersonalAssistantController(ILogger<PersonalAssistantController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{assistantKey}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<DashboardItem>> List(string assistantKey)
        {
            Console.WriteLine(assistantKey);
            return Engine.GetDashboard(assistantKey).ToList();
        }

    }
}
