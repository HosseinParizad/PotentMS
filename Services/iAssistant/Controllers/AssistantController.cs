using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PotentHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iAssistant.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AssistantController : ControllerBase
    {
        private readonly ILogger<AssistantController> _logger;

        public AssistantController(ILogger<AssistantController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //public IEnumerable<GoalItem> Get(string groupKey)
        //{
        //    return Engine.GetGoal(groupKey);
        //}

        [HttpGet]
        [Route("GetPresentation")]
        public IEnumerable<PresentItem> GetPresentation(string groupKey)
        {
            return Engine.GetPresentation(groupKey, "");
        }
    }
}
