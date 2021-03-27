using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KafkaHelper;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] GatewayMessage msg)
        {
            {
                var task = Producer.SendAMessage("task", JsonSerializer.Serialize(msg));
                task.GetAwaiter().GetResult();
                return StatusCode(StatusCodes.Status200OK);
            }
        }
    }
}