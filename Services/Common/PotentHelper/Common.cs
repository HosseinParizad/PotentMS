using System;
namespace PotentHelper
{
    public class MessageTopic
    {
        public const string TaskFeedback = "TaskFeedback";
        public const string Task = "Task";
        public const string Location = "Location";
        public const string Common = "Common";
        public const string Repeat = "Repeat";
        public const string RepeatFeedback = "RepeatFeedback";
        public const string PersonalAssistantFeedback = "PAFeedback";
    }

    public class FeedbackGroupNames
    {
        public const string Task = "FromTask";
        public const string Repeat = "FromRepeat";
        public const string PersonalAssistant = "FromPA";
    }

    public class KafkaEnviroment
    {
        //public const string preFix = "Production.";
        public const string preFix = "";
    }

    public static class MapAction
    {
        public static class Repeat
        {
            public const string RegisterRepeat = "registerRepeat";
        }

        public static class Task
        {
            public const string RepeatTask = "repeatTask";
        }
        
        public static class Location
        {
            public const string SetCurrentLocation = "setCurrentLocation";
        }
    }

}
