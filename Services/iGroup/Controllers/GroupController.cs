using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PotentHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iGroup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ILogger<GroupController> _logger;

        public GroupController(ILogger<GroupController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<GroupItem> Get(string groupKey)
        {
            return Engine.GetGroup(groupKey);
        }

        [HttpGet]
        [Route("GetGroupsTestOnly")]
        public IEnumerable<GroupItem> GetGroupsTestOnly()
        {
            return Engine.GetGroupsTestOnly();
        }

        [HttpGet]
        [Route("GetPresentation")]
        public IEnumerable<PresentItem> GetPresentation(string groupKey, string memberKey)
        {
            memberKey = memberKey == "" ? null : memberKey;
            groupKey = groupKey == "" ? null : groupKey;
            return Engine.GetGroupPresentation(groupKey, memberKey);
        }
    }
}
