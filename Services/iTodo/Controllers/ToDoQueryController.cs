using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [Route("Reset")]
        public void Reset()
        {
            Engine.Reset();
        }
    }
}
