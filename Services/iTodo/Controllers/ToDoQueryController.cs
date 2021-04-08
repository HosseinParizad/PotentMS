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
    public class ToDoQueryController : ControllerBase
    {
        private readonly ILogger<ToDoQueryController> _logger;

        public ToDoQueryController(ILogger<ToDoQueryController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<ToDoItem> Get()
        {
            var rng = new Random();
            var x = new ToDoItem("no desc", "no group");
            x.AssignedTo = "asghar";
            return new[] { x };
        }
    }
}
