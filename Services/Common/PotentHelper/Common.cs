using System;
using System.Collections.Generic;
using System.Linq;

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
        public const string RepeatFeedback = "RepeatFeedback";
        public const string TaskFeedback = "TaskFeedback";
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
            public static MapActionItem TestOnlyLocationChanged = Mai.SetName("TestOnlyLocationChanged");
            public static MapActionItem RegisterMember = Mai.SetName("RegisterMember");

            static MapActionItem Mai => MapActionItem.Instance("not used");
        }
        public static class Common
        {
            public static MapActionItem Reset = Mai.SetName("reset");

            static MapActionItem Mai => MapActionItem.Instance("");
        }

        public static class Repeat
        {
            public static MapActionItem RegisterRepeat = Mia.SetName("registerRepeat");

            static MapActionItem Mia => MapActionItem.Instance(MessageTopic.Repeat);
        }
        public static class RepeatFeedback
        {
            public static MapActionItem RepeatNewItem = Mia.SetName("RepeatNewItem");

            static MapActionItem Mia => MapActionItem.Instance(MessageTopic.RepeatFeedback);
        }


        public static class Task
        {
            public static MapActionItem NewTask = Mai.SetName("newTask");
            public static MapActionItem DelTask = Mai.SetName("delTask");
            public static MapActionItem CloseTask = Mai.SetName("closeTask");
            public static MapActionItem UpdateDescription = Mai.SetName("updateDescription");
            public static MapActionItem SetLocation = Mai.SetName("setLocation");
            public static MapActionItem SetDeadline = Mai.SetName("setDeadline");
            public static MapActionItem SetTag = Mai.SetName("setTag");
            public static MapActionItem AssignTask = Mai.SetName("assignTask");
            public static MapActionItem MoveTask = Mai.SetName("moveTask");
            //public static MapActionItem RepeatTask = Mai.SetName("repeatTask");

            static MapActionItem Mai => MapActionItem.Instance("Task");
        }

        public static class TaskFeedback
        {
            public static MapActionItem NewTaskAdded = Mai.SetName("New task has been added");
            public static MapActionItem NewTagAdded = Mai.SetName("New tag has been added");
            public static MapActionItem DeadlineUpdated = Mai.SetName("Deadline has been added");
            public static MapActionItem NewLocationAdded = Mai.SetName("New location has been added");
            public static MapActionItem TaskAssginedToMember = Mai.SetName("Task assgined to a member");
            public static MapActionItem updateTaskDescription = Mai.SetName("Task description has been added");
            public static MapActionItem moveTask = Mai.SetName("Task parent has been changed");
            public static MapActionItem TaskDeleted = Mai.SetName("Task has been deleted");
            public static MapActionItem CannotSetTag = Mai.SetName("Error: cannot set tag");
            public static MapActionItem CannotCloseTask = Mai.SetName("Error: cannot close tag");
            public static MapActionItem TaskClosed = Mai.SetName("Task has been closed");
            public static MapActionItem RepeatTask = Mai.SetName("Repeat task");
            public static MapActionItem CannotAddTask = Mai.SetName("Error: cannot add task");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.TaskFeedback);
        }

        public static class Group
        {
            public static MapActionItem NewGroup = Mai.SetName("newGroup");
            public static MapActionItem UpdateGroup = Mai.SetName("updGroup");
            public static MapActionItem NewMember = Mai.SetName("newMember");
            public static MapActionItem DeleteGroup = Mai.SetName("delGroup");
            public static MapActionItem DeleteMember = Mai.SetName("delMember");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.Group);
        }

        public static class GroupFeedback
        {
            public static MapActionItem CannotAddGroup = Mai.SetName("Error: cannot add Group");
            public static MapActionItem CannotUpdateGroup = Mai.SetName("Error: cannot update Group");
            public static MapActionItem CannotAddMember = Mai.SetName("Error: cannot add member");
            public static MapActionItem NewGroupAdded = Mai.SetName("New Group has been added");
            public static MapActionItem GroupDeleted = Mai.SetName("Group has been deleted");
            public static MapActionItem CannotFindGroup = Mai.SetName("Error: cannot find Group item");
            public static MapActionItem CannotFindMember = Mai.SetName("Error: cannot find member");
            public static MapActionItem NewMemberAdded = Mai.SetName("New Member has been added");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.GroupFeedback);
        }

        public static class Location
        {
            public static MapActionItem RegisterMember = Mai.SetName("RegisterMember");
            public static MapActionItem TestOnlyLocationChanged = Mai.SetName("TestOnlyLocationChanged");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.Location);
        }

        public static class LocationFeedback
        {
            public static MapActionItem LocationChanged = Mai.SetName("Member move to new location");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.LocationFeedback);
        }

        public static class Memory
        {
            public static MapActionItem NewMemory = Mai.SetName("newMemory");
            public static MapActionItem UpdateMemory = Mai.SetName("updateMemory");
            public static MapActionItem NewMemoryCategory = Mai.SetName("newMemoryCategory");
            public static MapActionItem DelMemory = Mai.SetName("delMemory");
            public static MapActionItem LearntMemory = Mai.SetName("lrnMemory");
            public static MapActionItem HintMemory = Mai.SetName("hintMemory");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.Memory);
        }

        public static class MemoryFeedback
        {
            public static MapActionItem NewMemoryAdded = Mai.SetName("New memory has been added");
            public static MapActionItem MemoryDeleted = Mai.SetName("Memory has been deleted");
            public static MapActionItem CannotFindMemory = Mai.SetName("Error: cannot find Memory item");
            public static MapActionItem CannotAddMemory = Mai.SetName("Error: cannot add Memory");

            static MapActionItem Mai => MapActionItem.Instance("MemoryFeedback");
        }


        public static class Goal
        {
            public static MapActionItem NewGoal = Mai.SetName("newGoal");
            public static MapActionItem UpdateGoal = Mai.SetName("updateGoal");
            public static MapActionItem DelGoal = Mai.SetName("delGoal");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.Goal);
        }

        public static class GoalFeedback
        {
            public static MapActionItem CannotAddGoal = Mai.SetName("Error: cannot add Goal");
            public static MapActionItem CannotUpdateGoal = Mai.SetName("Error: cannot update Goal");
            public static MapActionItem NewGoalAdded = Mai.SetName("New Goal has been added");
            public static MapActionItem GoalDeleted = Mai.SetName("Goal has been deleted");
            public static MapActionItem CannotFindGoal = Mai.SetName("Error: cannot find Goal item");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.GoalFeedback);
        }

        public static class Time
        {
            public static MapActionItem Start = Mai.SetName("start");
            public static MapActionItem Pause = Mai.SetName("pause");
            public static MapActionItem Done = Mai.SetName("done");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.Time);
        }
        public static class TimeFeedback
        {
            public static MapActionItem TimeableStarted = Mai.SetName("Task has been started");
            public static MapActionItem TimeablePaused = Mai.SetName("Task has been paused");
            public static MapActionItem TimeableDone = Mai.SetName("Task has been done");
            public static MapActionItem CannotStartTimeable = Mai.SetName("Error: cannot start time");
            public static MapActionItem CannotPauseTimeable = Mai.SetName("Error: cannot pause time");
            public static MapActionItem CannotDoneTimeable = Mai.SetName("Error: cannot done time");

            static MapActionItem Mai => MapActionItem.Instance(MessageTopic.TimeFeedback);
        }
    }

    public class MapActionItem
    {
        public MapActionItem(string topic)
        {
            Topic = topic;
        }

        public static MapActionItem Instance(string topic)
        {
            return new MapActionItem(topic);
        }

        public string Topic { get; set; }
        public string Name { get; set; }
    }

    public static class CreateExtentionPlace
    {
        public static MapActionItem SetName(this MapActionItem mai, string name)
        {
            mai.Name = name;
            return mai;
        }

        public static List<string> Topics(this List<MapBinding> list)
        {
            return list.Select(l => l.Topic).Distinct().Where(t => t != "").ToList();
        }

        public static bool HasAction(this List<MapBinding> list, string action)
        {
            return list.Any(l => l.ActionName == action);
        }
    }

    public class MapBinding
    {
        public MapBinding(MapActionItem mapActionItem, Action<dynamic, dynamic> act)
        {
            MapActionItem = mapActionItem;
            Act = act;
        }

        public string ActionName => MapActionItem.Name;
        public string Topic => MapActionItem.Topic;
        public MapActionItem MapActionItem { get; }
        public Action<dynamic, dynamic> Act { get; set; }
    }
}
