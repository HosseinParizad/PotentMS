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
        public static List<string> PAFeedbackQueue { get; private set; } = new List<string>();

        [HttpPost]
        public IActionResult Post([FromBody] Msg msg)
        {
            var task = ProducerHelper.SendAMessage(MessageTopic.Task, JsonSerializer.Serialize(msg));
            task.GetAwaiter().GetResult();
            return StatusCode(StatusCodes.Status200OK);
        }

        internal static void Reset(string arg1, string arg2)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("Location")]
        public IActionResult PostLocation([FromBody] Msg msg)
        {
            Console.WriteLine("||||||||||||||||||||||||||||||||||||||2|3|4|5|6|7|8|9|||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||");
            var task = ProducerHelper.SendAMessage(MessageTopic.Location, JsonSerializer.Serialize(msg));
            task.GetAwaiter().GetResult();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("Repeat")]
        public IActionResult PostRepeat([FromBody] Msg msg)
        {
            var task = ProducerHelper.SendAMessage(MessageTopic.Repeat, JsonSerializer.Serialize(msg));
            task.GetAwaiter().GetResult();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("Common")]
        public IActionResult PostCommon([FromBody] Msg msg)
        {
            var task = ProducerHelper.SendAMessage(MessageTopic.Common, JsonSerializer.Serialize(msg));
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

        [HttpGet]
        [Route("PAFeedback")]
        public IEnumerable<string> GetPA()
        {
            {
                return PAFeedbackQueue;
            }
        }

        [HttpPost]
        [Route("DeleteTopics")]
        public IActionResult DeleteTopics()
        {
            var topicNameList = new List<string> { "Repeat", "PAFeedback", "Task", "Location", "Common", "RepeatFeedback" };
            //foreach (var item in topicNameList.ToArray())
            //{
            //    topicNameList.Add(KafkaEnviroment.preFix + item);
            //}
            ConsumerHelper.deleteTopics(topicNameList);
            return StatusCode(StatusCodes.Status200OK);
        }


        internal static void MessageReceived(Feedback feedback)
        {
            FeedbackQueue.Add(feedback.Content);
        }

        internal static void PAMessageReceived(Feedback feedback)
        {
            PAFeedbackQueue.Add(feedback.Content);
        }


    }
}