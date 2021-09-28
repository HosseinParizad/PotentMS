using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PotentHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iLocation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILogger<LocationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<dynamic> Get(string groupKey)
        {
            //return Engine.GetGroup(groupKey);
            return Enumerable.Empty<dynamic>();
        }

        //[HttpGet]
        //[Route("GetPresentation")]
        //public IEnumerable<PresentItem> GetPresentation(string groupKey, string memberKey)
        //{
        //    return Engine.GetGroupPresentation(groupKey, memberKey);
        //}
    }
}
