using System;
namespace PotentHelper
{
    public class MessageTopic
    {
        public const string TaskFeedback = "TaskFeedback";
        public const string Task = "Task";
        public const string Location = "Location";
        public const string Common = "Common";
    }

    public class FeedbackGroupNames
    {
        public const string Task = "FromTask";
    }

    public class KafkaEnviroment
    {
        //public const string preFix = "Production.";
        public const string preFix = "";
    }
}
