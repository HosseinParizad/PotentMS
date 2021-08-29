using System.Diagnostics;

namespace PotentHelper
{
    public class Feedback : IMessageContract
    {
        public Feedback(FeedbackType type, string name, string action, dynamic metadata, dynamic content)
        {
            Type = type;
            Action = action;
            Metadata = metadata;
            Content = content;
            Name = name;
        }

        public FeedbackType Type { get; set; }
        public string Action { get; set; }
        public string Name { get; set; }
        public dynamic Metadata { get; set; }
        public dynamic Content { get; set; }
    }

    public enum FeedbackType
    {
        Success,
        Apply,
        Info,
        Error
    }

    public class FeedbackActions
    {
        public const string NewTaskAdded = "New task has been added";
        public const string NewTagAdded = "New tag has been added";
        public const string DeadlineUpdated = "Deadline has been added";
        public const string NewLocationAdded = "New location has been added";
        public const string NewGroupAdded = "New group/member has been added";
        public const string NewGoalAdded = "New goal has been added";
        public const string TaskAssginedToMember = "Task assgined to a member";
        public const string updateTaskDescription = "Task description has been added";
        public const string moveTask = "Task parent has been changed";
        public const string TaskDeleted = "Task has been deleted";
        public const string GoalDeleted = "Goal has been deleted";
        public const string CannotSetTag = "Error: cannot set tag";
        public const string CannotCloseTask = "Error: cannot close tag";
        public const string TaskClosed = "Task has been closed";
        public const string TaskStarted = "Task has been started";
        public const string TaskPaused = "Task has been paused";
        public const string RepeatTask = "Repeat task";
        public const string CannotAddTask = "Error: cannot add task";
        public const string CannotAddMemory = "Error: cannot add Memory";
        public const string NewMemoryAdded = "New memory has been added";
        public const string MemoryDeleted = "Memory has been deleted";
        public const string CannotFindMemory = "Error: cannot find Memory item";
    }
}
