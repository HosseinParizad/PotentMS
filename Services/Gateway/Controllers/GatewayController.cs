using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PotentHelper;
using System.Collections.Generic;

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
            var msgT = Helper.DeserializeObject<dynamic>(System.Text.Json.JsonSerializer.Serialize(msg)); //Todo:remove

            var task = ProducerHelper.SendAMessage(MessageTopic.Task, JsonConvert.SerializeObject(msgT));
            task.GetAwaiter().GetResult();
            return StatusCode(StatusCodes.Status200OK);
        }

        internal static void Reset(dynamic arg1, dynamic arg2)
        {

        }

        [HttpPost]
        [Route("Location")]
        public IActionResult PostLocation([FromBody] Msg msg)
        {
            var msgT = Helper.DeserializeObject<dynamic>(System.Text.Json.JsonSerializer.Serialize(msg)); //Todo:remove

            var task = ProducerHelper.SendAMessage(MessageTopic.Location, JsonConvert.SerializeObject(msgT));
            task.GetAwaiter().GetResult();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("Repeat")]
        public IActionResult PostRepeat([FromBody] Msg msg)
        {
            var msgT = Helper.DeserializeObject<dynamic>(System.Text.Json.JsonSerializer.Serialize(msg)); //Todo:remove

            var task = ProducerHelper.SendAMessage(MessageTopic.Repeat, JsonConvert.SerializeObject(msgT));
            task.GetAwaiter().GetResult();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("Common")]
        public IActionResult PostCommon([FromBody] Msg msg)
        {
            var msgT = Helper.DeserializeObject<dynamic>(System.Text.Json.JsonSerializer.Serialize(msg)); //Todo:remove

            var task = ProducerHelper.SendAMessage(MessageTopic.Common, JsonConvert.SerializeObject(msgT));
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
            var topicNameList = new List<string> { "Repeat", "PAFeedback", "Task", "Location", "Common", "RepeatFeedback", "TaskFeedback" };
            foreach (var item in topicNameList.ToArray())
            {
                topicNameList.Add(KafkaEnviroment.preFix + item);
            }
            ConsumerHelper.deleteTopics(topicNameList);
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        [Route("DeleteFeedback")]
        public IActionResult DeleteFeedback()
        {
            var topicNameList = new List<string> { "PAFeedback", "RepeatFeedback", "TaskFeedback" };
            foreach (var item in topicNameList.ToArray())
            {
                topicNameList.Add(KafkaEnviroment.preFix + item);
            }
            ConsumerHelper.deleteTopics(topicNameList);
            return StatusCode(StatusCodes.Status200OK);
        }


        internal static void MessageReceived(Feedback feedback)
        {
            FeedbackQueue.Add(JsonConvert.SerializeObject(feedback.Content));
        }

        internal static void PAMessageReceived(Feedback feedback)
        {
            PAFeedbackQueue.Add(JsonConvert.SerializeObject(feedback.Content));
        }


    }
}