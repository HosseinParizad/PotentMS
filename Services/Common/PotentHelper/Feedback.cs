using System.Diagnostics;

namespace PotentHelper
{
    public class Feedback : IMessageContract
    {
        public Feedback(FeedbackType type, string name, string action, string key, string content)
        {
            Type = type;
            Action = action;
            Key = key;
            Content = content;
            Name = name;
        }

        public FeedbackType Type { get; set; }
        public string Key { get; set; }
        public string Content { get; set; }
        public string Action { get; set; }
        public string Name { get; set; }
    }

    public enum FeedbackType
    {
        Success,
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
        public const string TaskDeleted = "Task has been deleted";
        public const string GoalDeleted = "Goal has been deleted";

        public const string CannotSetTag = "Error: cannot set tag";
        public const string CannotCloseTask = "Error: cannot close tag";
    }
}
