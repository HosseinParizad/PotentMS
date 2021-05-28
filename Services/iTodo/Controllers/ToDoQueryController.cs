using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace iTodo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoQueryController : ControllerBase
    {
        private readonly ILogger<TodoQueryController> _logger;

        public TodoQueryController(ILogger<TodoQueryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<TodoItem> Get(string groupKey)
        {
            return Engine.GetTask(groupKey);
        }

        [HttpGet]
        [Route("GroupQuery")]
        public IEnumerable<GroupItem> GroupQuery(string groupKey)
        {
            return Engine.GetGroup(groupKey);
        }

        [HttpGet]
        [Route("Sort")]
        public string GetSort()
        {
            return Engine.GetSort;
        }

        [HttpGet("/GetTaskByGroupTag/{groupKey}/{Tag}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TodoItem>> GetTaskByGroupTag(string groupKey, string tag)
        {
            return Engine.GetTaskByGroupTag(groupKey, tag).ToList();
        }

        [HttpGet("/GetTaskWhenMoveToLocation/{groupKey}/{Tag}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TodoItem>> GetTaskWhenMoveToLocation(string groupKey, string tag)
        {
            return Engine.GetTaskWhenMoveToLocation(groupKey, tag).ToList();
        }
    }
}
