using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PotentHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iMemory.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MemoryController : ControllerBase
    {
        private readonly ILogger<MemoryController> _logger;

        public MemoryController(ILogger<MemoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<MemoryItem> Get(string groupKey, string memberKey)
        {
            return Engine.GetMemory(groupKey, memberKey);
        }

        [HttpGet]
        [Route("GetPresentation")]
        public IEnumerable<PresentItem> GetPresentation(string groupKey, string memberKey)
        {
            return Engine.GetMemoryPresentation(groupKey, memberKey, "");
        }
    }
}
