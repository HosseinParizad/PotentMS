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
            var task = ProducerHelper.SendAMessage("task", JsonSerializer.Serialize(msg));
            task.GetAwaiter().GetResult();
            return StatusCode(StatusCodes.Status200OK);
        }


        [HttpPost]
        [Route("Location")]
        public IActionResult PostLocation([FromBody] Msg msg)
        {
            Console.WriteLine("||||||||||||||||||||||||||||||||||||||2|3|4|5|6|7|8|9|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            var task = ProducerHelper.SendAMessage("location", JsonSerializer.Serialize(msg));
            task.GetAwaiter().GetResult();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpGet]
        [Route("Feedback")]
        public IEnumerable<string> Get()
        {
            {
                return FeedbackQueue;
            }
        }

        internal static void MessageReceived(Feedback feedback)
        {
            FeedbackQueue.Add(feedback.Content);
        }

    }
}