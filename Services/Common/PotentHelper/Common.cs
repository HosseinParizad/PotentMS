using System;
namespace PotentHelper
{
    public class MessageTopic
    {

        public const string Task = "Task";
        public const string Location = "Location";
        public const string Common = "Common";
        public const string Memory = "Memory";
        public const string Repeat = "Repeat";
        public const string Goal = "Goal";
        public const string Group = "Group";
        public const string Time = "Time";
        public const string MemoryFeedback = "MemoryFeedback";
        public const string TaskFeedback = "TaskFeedback";
        public const string RepeatFeedback = "RepeatFeedback";
        public const string PersonalAssistantFeedback = "PAFeedback";
        public const string GoalFeedback = "GoalFeedback";
        public const string GroupFeedback = "GroupFeedback";
        public const string TimeFeedback = "TimeFeedback";
        public const string LocationFeedback = "LocationFeedback";
    }

    public class FeedbackGroupNames
    {
        public const string All = "All";
        public const string Task = "FromTask";
        public const string Memory = "FromMemory";
        public const string Goal = "FromGoal";
        public const string Group = "FromGroup";
        public const string Repeat = "FromRepeat";
        public const string Time = "FromTime";
        public const string PersonalAssistant = "FromPA";
    }

    public class KafkaEnviroment
    {
        public static string preFix => TempPrefix ?? "Production.Me."; // test feature

        public static string TempPrefix { get; set; } = "";
    }

    public static class MapAction
    {
        public static class Assistant
        {
            public const string TestOnlyLocationChanged = "TestOnlyLocationChanged";

            public static string RegisterMember = "RegisterMember";
        }

        public static class Repeat
        {
            public const string RegisterRepeat = "registerRepeat";
        }

        public static class Task
        {
            public const string NewTask = "newTask";
            public const string DelTask = "delTask";
            public const string CloseTask = "closeTask";
            public const string UpdateDescription = "updateDescription";
            public const string SetLocation = "setLocation";
            public const string SetDeadline = "setDeadline";
            public const string SetTag = "setTag";
            public const string AssignTask = "assignTask";
            public const string MoveTask = "moveTask";
            public const string RepeatTask = "repeatTask";
        }

        public static class Group
        {
            public const string NewGroup = "newGroup";
            public const string UpdateGroup = "updGroup";
            public const string NewMember = "newMember";
            public const string DeleteGroup = "delGroup";
            public const string DeleteMember = "delMember";
        }

        public static class Location
        {
            public const string RegisterMember = "RegisterMember";
            public const string TestOnlyLocationChanged = "TestOnlyLocationChanged";
            //public const string SetCurrentLocation = "setCurrentLocation";
        }

        public static class Memory
        {
            public const string NewMemory = "newMemory";
            public const string NewMemoryCategory = "newMemoryCategory";
            public const string DelMemory = "delMemory";
            public const string LearntMemory = "lrnMemory";
        }
        public static class Goal
        {
            public const string NewGoal = "newGoal";
            public const string UpdateGoal = "updateGoal";
            public const string DelGoal = "delGoal";
        }
        public static class Time
        {
            public const string Start = "start";
            public const string Pause = "pause";
            public const string Done = "done";
        }
    }

}
