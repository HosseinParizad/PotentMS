using System.Diagnostics;

namespace PotentHelper
{
    //public class Feedback : Msg, IMessageContract
    //{
    //    public Feedback(string action, dynamic metadata, dynamic content) : base(action, metadata, content)
    //    {
    //    }

    //    public Feedback(MsgType type, string action, dynamic metadata, dynamic content) : base(type, action, metadata, content)
    //    {
    //    }

    //    //public Feedback(MsgType type, string action, dynamic metadata, dynamic content) : base(type, action, metadata, content)
    //    //{ }

    //    public MsgType Type { get; set; }
    //    public string Action { get; set; }
    //    public dynamic Metadata { get; set; }
    //    public dynamic Content { get; set; }
    //}
    public class Feedback : Msg
    {
        public Feedback(MsgType type, string action, dynamic metadata, dynamic content) : base(type, action, (object)metadata, (object)content)
        {
        }
    }

    public enum MsgType
    {
        Command,
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
        public const string TaskAssginedToMember = "Task assgined to a member";
        public const string updateTaskDescription = "Task description has been added";
        public const string moveTask = "Task parent has been changed";
        public const string TaskDeleted = "Task has been deleted";
        public const string CannotSetTag = "Error: cannot set tag";
        public const string CannotCloseTask = "Error: cannot close tag";
        public const string TaskClosed = "Task has been closed";
        public const string RepeatTask = "Repeat task";
        public const string CannotAddTask = "Error: cannot add task";
        public const string CannotAddMemory = "Error: cannot add Memory";
        public const string NewMemoryAdded = "New memory has been added";
        public const string MemoryDeleted = "Memory has been deleted";
        public const string CannotFindMemory = "Error: cannot find Memory item";
        public const string CannotAddGoal = "Error: cannot add Goal";
        public const string CannotUpdateGoal = "Error: cannot update Goal";
        public const string NewGoalAdded = "New Goal has been added";
        public const string GoalDeleted = "Goal has been deleted";
        public const string CannotFindGoal = "Error: cannot find Goal item";

        //Group

        public const string CannotAddGroup = "Error: cannot add Group";
        public const string CannotUpdateGroup = "Error: cannot update Group";
        public const string CannotAddMember = "Error: cannot add member";
        public const string NewGroupAdded = "New Group has been added";
        public const string GroupDeleted = "Group has been deleted";
        public const string CannotFindGroup = "Error: cannot find Group item";
        public const string CannotFindMember = "Error: cannot find member";
        public const string NewMemberAdded = "New Member has been added";

        // Time
        public const string TimeableStarted = "Task has been started";
        public const string TimeablePaused = "Task has been paused";
        public const string TimeableDone = "Task has been done";
        public const string CannotStartTimeable = "Error: cannot start time";
        public const string CannotPauseTimeable = "Error: cannot pause time";
        public const string CannotDoneTimeable = "Error: cannot done time";

        // Location
        public const string LocationChanged = "Member move to new location";
    }
}
