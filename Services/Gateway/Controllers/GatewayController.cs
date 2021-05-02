using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PotentHelper;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        public static List<string> FeedbackQueue { get; private set; } = new List<string>();

        [HttpPost]
        public IActionResult Post([FromBody] Msg msg)
        {
            {
                if (msg.Action == "Reset")
                {
                    FeedbackQueue = new List<string>();
                }
                var task = ProducerHelper.SendAMessage("task", JsonSerializer.Serialize(msg));
                task.GetAwaiter().GetResult();
                return StatusCode(StatusCodes.Status200OK);
            }
        }

        [HttpGet]
        [Route("Feedback")]
        public IEnumerable<string> Get()
        {
            {
                return FeedbackQueue;
            }
        }

        internal static void MessageReceived(string msg)
        {
            FeedbackQueue.Add(msg);
        }

    }
}