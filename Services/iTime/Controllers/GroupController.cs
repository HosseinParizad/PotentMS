using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PotentHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iTime.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimeController : ControllerBase
    {
        private readonly ILogger<TimeController> _logger;

        public TimeController(ILogger<TimeController> logger)
        {
            _logger = logger;
        }

        //[HttpGet]
        //public IEnumerable<TimeItem> Get(string TimeKey)
        //{
        //    return Engine.GetTime(TimeKey);
        //}

        //[HttpGet]
        //[Route("GetTimesTestOnly")]
        //public IEnumerable<TimeItem> GetTimesTestOnly()
        //{
        //    return Engine.GetTimesTestOnly();
        //}

        [HttpGet]
        [Route("GetPresentation")]
        public IEnumerable<PresentItem> GetPresentation(string groupKey, string memberKey)
        {
            return Engine.GetTimePresentation(groupKey, memberKey);
        }
    }
}
