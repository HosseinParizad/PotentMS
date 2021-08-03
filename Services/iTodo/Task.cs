using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PotentHelper;

namespace iTodo
{
    internal class Engine
    {
        #region CreateNewTask 

        public static void CreateNewTask(dynamic metadata, dynamic content)
        {
            var description = content.GetProperty("Description").ToString();
            var parentId = content.GetProperty("ParentId").ToString();
            var id = metadata.GetProperty("ReferenceKey").ToString();
            CreateGroupIfNotExists(GetValue(metadata, "GroupKey"));
            AddTask(id, GetValue(metadata, "GroupKey"), description, parentId);
        }

        public static string GetValue(dynamic metadata,string prop)
        {
            return metadata.GetProperty(prop).ToString();
        }

        static void AddTask(string id, string groupKey, dynamic description, dynamic parentId)
        {
            var newItem = new TodoItem();
            newItem.Id = id;
            newItem.GroupKey = groupKey;
            newItem.Sequence = Todos.Count;
            newItem.Description = description;
            newItem.Kind = TodoType.Task;
            if (!string.IsNullOrEmpty(parentId))
            {
                newItem.ParentId = parentId;
            }
            AddTask(newItem);
        }

        static void AddTask(TodoItem newItem)
        {
            Todos.Add(newItem);

            var dataToSend = new { Id = newItem.Id, Text = newItem.Description, ParentId = newItem.ParentId };
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewTaskAdded, groupkey: newItem.GroupKey, content: dataToSend);
        }

        public static void CreateNewGoal(dynamic metadata, dynamic content)
        {
            var newItem = new TodoItem();
            newItem.Id = metadata.GetProperty("ReferenceKey").ToString();
            newItem.Description = content.GetProperty("Description").ToString();
            newItem.GroupKey = GetValue(metadata, "GroupKey");
            newItem.Sequence = Todos.Count;
            newItem.Kind = TodoType.Goal;
            Todos.Add(newItem);
            var dataToSend = new { Id = newItem.Id, Goal = newItem.Description };
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewGoalAdded, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);
            CreateGroupIfNotExists(GetValue(metadata, "GroupKey"));
        }

        #endregion

        #region UpdateDescription 

        public static void UpdateDescription(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            var description = content.GetProperty("Description").ToString();
            FindById(GetValue(metadata, "GroupKey"), id).Description = description;
            var dataToSend = new { Id = id, Description = description };
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.updateTaskDescription, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);
        }

        #endregion

        #region SetDeadline 

        public static void SetDeadline(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            var deadline = content.GetProperty("Deadline").GetDateTimeOffset();
            TodoItem todo = FindById(GetValue(metadata, "GroupKey"), id);
            todo.Deadline = deadline;
            var dataToSend = new { Id = id, Text = todo.Description, Deadline = deadline };
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.DeadlineUpdated, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);
        }

        #endregion

        #region SetTag 

        public static void SetTag(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            var tag = content.GetProperty("Tag").ToString();
            var tagKey = content.GetProperty("TagKey").ToString();
            var todo = FindById(GetValue(metadata, "GroupKey"), id);
            if (todo == null)
            {
                SendFeedbackMessage(type: FeedbackType.Error, action: FeedbackActions.CannotSetTag, groupkey: GetValue(metadata, "GroupKey"), content: "Cannot find Todo item to assgin tag!");
            }
            else
            {
                Console.WriteLine(tag);
                UpdateTags(todo, GetValue(metadata, "GroupKey"), tag, tagKey);
            }
        }

        #endregion

        #region UpdateTags

        static void UpdateTags(TodoItem todo, string groupKey, string allTag, string tagKey)
        {
            var parent = todo.Tags.SingleOrDefault(t => t.TagParentKey == tagKey);
            if (parent == null)
            {
                parent = new TagItem { TagParentKey = tagKey, Value = new List<string>() };
                todo.Tags.Add(parent);
            }
            foreach (var tag in allTag.Split(",").Distinct())
            {
                if (!parent.Value.Contains(tag))
                {
                    parent.Value.Add(tag);
                    SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewTagAdded, groupkey: groupKey, content: tag);
                }
            }
        }


        #endregion

        #region CloseTask 

        public static void CloseTask(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            var task = FindById(GetValue(metadata, "GroupKey"), id);
            if (task != null)
            {
                task.Status = TodoStatus.Close;
                var dataToSend = new { Id = id };
                SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.TaskClosed, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);
            }
            else
            {
                SendFeedbackMessage(type: FeedbackType.Error, action: FeedbackActions.CannotCloseTask, groupkey: GetValue(metadata, "GroupKey"), content: "Cannot find Todo item to close task!");
            }
        }

        #endregion

        #region StartTask

        public static void StartTask(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            var task = FindById(GetValue(metadata, "GroupKey"), id);
            if (task != null)
            {
                Func<TodoItem, bool> condition = (t) => { return t.Status == TodoStatus.start; };
                var startingtask = FindFirstByCondition(GetValue(metadata, "GroupKey"), condition);
                if (startingtask != null)
                {
                    PauseTask(GetValue(metadata, "GroupKey"), startingtask.Id, startingtask);
                }

                task.Status = TodoStatus.start;
                TimeLog.Add(new TimeItem { Id = Guid.NewGuid().ToString(), ActionTime = DateTimeOffset.Now, TodoId = id, Status = TimeActionStatus.Start });

                var dataToSend = new { Id = id };
                SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.TaskStarted, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);
            }
        }

        #endregion

        #region PauseTask

        public static void PauseTask(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            var task = FindById(GetValue(metadata, "GroupKey"), id);
            if (task != null && task.Status == TodoStatus.start)
            {
                PauseTask(GetValue(metadata, "GroupKey"), id, task);
            }
        }

        static void PauseTask(string groupKey, dynamic id, dynamic task)
        {
            task.Status = TodoStatus.pause;
            TimeLog.Add(new TimeItem { Id = Guid.NewGuid().ToString(), ActionTime = DateTimeOffset.Now, TodoId = id, Status = TimeActionStatus.Pause });
            var dataToSend = new { Id = id };
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.TaskPaused, groupkey: groupKey, content: dataToSend);
        }

        #endregion

        #region DeleteTask 

        public static void DeleteTask(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            var task = FindById(GetValue(metadata, "GroupKey"), id);
            if (task != null)
            {
                Todos.Remove(task);

                var dataToSend = new { Id = id };
                if (task.Kind == TodoType.Task)
                {
                    SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.TaskDeleted, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);
                }
                else if (task.Kind == TodoType.Goal)
                {
                    SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.GoalDeleted, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);
                }
            }
            else
            {
                //SendFeedbackMessage(type: FeedbackType.Error, groupKey: groupKey, id: id, content: "Cannot find task!", originalRequest: "CloseTask");
            }
        }

        #endregion

        #region AssignTask

        public static void AssignTask(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            var assignTo = content.GetProperty("AssignTo").ToString();
            TodoItem task = FindById(GetValue(metadata, "GroupKey"), id);
            if (task != null)
            {
                task.AssignedTo = assignTo;
            }
            var dataToSend = new { Id = id, MemberKey = assignTo };
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.TaskAssginedToMember, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);
        }

        #endregion

        #region SetLocation 

        public static void SetLocation(dynamic metadata, dynamic content)
        {
            var id = content.GetProperty("Id").ToString();
            string location = content.GetProperty("Location").ToString();
            TodoItem todo = FindById(GetValue(metadata, "GroupKey"), id);
            if (todo == null)
            {
                //SendFeedbackMessage(type: FeedbackType.Error, groupKey: groupKey, id: id, content: "Cannot find!", originalRequest: "SetLocation");
            }
            else
            {
                todo.Locations.AddRange(location.Split(","));
                var dataToSend = new { Id = id, Location = location };
                SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewLocationAdded, groupkey: GetValue(metadata, "GroupKey"), content: dataToSend);

            }
        }

        #endregion

        #region NewGroup 

        public static void NewGroup(dynamic metadata, dynamic content)
        {
            var groupKey = GetValue(metadata, "GroupKey");
            Groups.Add(CreateNewGroup(groupKey, groupKey));
        }

        //static GroupItem CreateNewGroup(string groupKey, string member) => new GroupItem { Group = groupKey, Member = member, Tags = new List<TagSetting>() };

        static GroupItem CreateNewGroup(string groupKey, string member)
        {
            var dataToSend = new { GroupKey = groupKey, MemberKey = member };
            Console.WriteLine("******8****************88888888888888888************************");
            Console.WriteLine(dataToSend);
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewGroupAdded, groupkey: groupKey, content: dataToSend);
            return new GroupItem { Group = groupKey, Member = member, Tags = new List<TagSetting>() };
        }

        #endregion

        #region NewMember 

        public static void NewMember(dynamic metadata, dynamic content)
        {
            var newMember = content.GetProperty("NewMember").ToString();
            CreateGroupIfNotExists(newMember);
            Groups.Add(CreateNewGroup(GetValue(metadata, "GroupKey"), newMember));
        }

        static void CreateGroupIfNotExists(string groupKey)
        {
            if (!Groups.Any(g => g.Group == groupKey))
            {
                Groups.Add(CreateNewGroup(groupKey, groupKey));
            }
        }

        #endregion

        #region RepeatTask

        public static void RepeatTask(Feedback feedback)
        {
            var id = feedback.Content.GetProperty("Id").ToString();
            var date = DateTimeOffset.Parse(feedback.Content.GetProperty("LastGeneratedTime").ToString());
            var hours = int.Parse(feedback.Content.GetProperty("Hours").ToString());
            var dateStr = " (" + date.Date.ToShortDateString() + ")";

            TodoItem task = Todos.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                AddTaskAndChildren(task, task.ParentId, dateStr, hours);
            }
        }

        static void AddTaskAndChildren(TodoItem task, string parentId, string date, int hours)
        {
            var ctask = task.Clone();
            ctask.ParentId = parentId;
            ctask.Description += date;
            ctask.Deadline = task.Deadline?.AddHours(hours);
            ctask.Status = TodoStatus.Active;
            AddTask(ctask);
            foreach (var child in Todos.Where(t => t.ParentId == task.Id).ToArray())
            {
                AddTaskAndChildren(child, ctask.Id, date, hours);
            }
        }

        #endregion

        #region GetTask 

        public static IEnumerable<TodoItem> GetTask(string member)
        {
            if (member == "All")
            {
                return Todos;
            }
            return Todos.Where(i => i.Status != TodoStatus.Close && (i.AssignedTo ?? i.GroupKey) == member).OrderBy(t => MemberCurrentLocation.ContainsKey(member) && MemberCurrentLocation[member].Split(",").Any(l => t.Locations?.Contains(l) ?? false) ? 0 : 1).ThenBy(t => t.Sequence);
        }

        #endregion

        #region GetGroup

        public static IEnumerable<GroupItem> GetGroup(string groupKey)
        {
            if (groupKey == "All")
            {
                return Groups;
            }

            return Groups.Where(i => i.Group == groupKey);
        }

        #endregion

        #region GetTaskByGroupTag

        internal static IEnumerable<TodoItem> GetTaskByGroupTag(string groupKey, string tag)
        {
            return Todos.Where(i => i.Status != TodoStatus.Close && (i.AssignedTo ?? i.GroupKey) == groupKey && i.Tags.Any(t => t.Value.Contains(tag)));
        }

        #endregion

        #region GetTaskWhenMoveToLocation

        internal static IEnumerable<TodoItem> GetTaskWhenMoveToLocation(string groupKey, string tag)
        {
            return Todos.Where(i => i.Status != TodoStatus.Close && (i.AssignedTo ?? i.GroupKey) == groupKey && i.Locations.Any(l => l.IndexOf(tag) > -1));
        }

        #endregion

        #region Implement


        static void SendFeedbackMessage(FeedbackType type, string action, string groupkey, dynamic content)
        {
            Console.WriteLine($"{type}, {action}, {groupkey}, {content}");
            ProducerHelper.SendAMessage(MessageTopic.TaskFeedback, JsonSerializer.Serialize(
                new Feedback(type: type, name: FeedbackGroupNames.Task, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content)
                )).GetAwaiter().GetResult();
        }

        static TodoItem FindById(string groupKey, dynamic id) => Todos.SingleOrDefault(t => t.GroupKey == groupKey && t.Id == id);
        static TodoItem FindFirstByCondition(string groupKey, Func<TodoItem, bool> condition) => Todos.FirstOrDefault(t => t.GroupKey == groupKey && condition(t));

        static List<TodoItem> Todos = new List<TodoItem>();
        static List<TimeItem> TimeLog = new List<TimeItem>();

        static List<GroupItem> Groups { get; set; } = new List<GroupItem>();

        static Dictionary<string, string> MemberCurrentLocation { get; set; } = new Dictionary<string, string>();

        public static string GetSort => Sort;
        static string Sort = "";

        #endregion

        #region Location actions 

        public static void SetCurrentLocation(dynamic metadata, dynamic content)
        {
            var member = content.GetProperty("Member").ToString();
            string location = content.GetProperty("Location").ToString();
            if (MemberCurrentLocation.TryGetValue(member, out string locations))
            {
                MemberCurrentLocation[member] = string.Join(",", locations.Split(",").Union(location.Split(",")).Distinct());
            }
            else
            {
                MemberCurrentLocation.Add(member, location);
            }
        }

        #endregion

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            Todos = new List<TodoItem>();
            Groups = new List<GroupItem>();
            TimeLog = new List<TimeItem>();
            MemberCurrentLocation = new Dictionary<string, string>();
        }

        #endregion
    }

    #region Classes

    public class TodoItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string GroupKey { get; set; }
        public string AssignedTo { get; set; }
        public DateTimeOffset? Deadline { get; set; }
        public int Sequence { get; set; }
        public List<string> Locations { get; set; } = new List<string>();
        public List<TagItem> Tags { get; set; } = new List<TagItem>();
        public TodoStatus Status { get; set; }
        public string ParentId { get; set; }
        public TodoType Kind { get; set; }
        public List<TodoItem> TodoItems
        {
            get => todoItems ??= new List<TodoItem>();
            set
            {
                todoItems = value;
                foreach (var item in todoItems.Where(i => i.ParentId != ParentId))
                {
                    item.ParentId = ParentId;
                }
            }
        }
        List<TodoItem> todoItems;

        public TodoItem Clone()
        {
            TodoItem todoItem = (TodoItem)MemberwiseClone();
            todoItem.Locations = Locations;
            todoItem.Tags = Tags;
            todoItem.Id = Guid.NewGuid().ToString();
            return todoItem;
        }
    }

    public class TimeItem
    {
        public string Id { get; set; }
        public string TodoId { get; set; }
        public DateTimeOffset ActionTime { get; set; }
        public TimeActionStatus Status { get; set; }
    }

    public class GroupItem
    {
        public string Group { get; set; }
        public string Member { get; set; }
        public List<TagSetting> Tags { get; set; } = new List<TagSetting>();
    }

    public class TagSetting
    {
        public string Key { get; set; }
        public string Caption { get; set; }
        public string Values { get; set; }
    }

    public class TagItem
    {
        public string TagParentKey { get; set; }
        public List<string> Value { get; set; }
    }

    public enum TodoStatus
    {
        Active,
        Close,
        start,
        pause
    }

    public enum TodoType
    {
        Goal,
        Category,
        Task
    }

    public enum TimeActionStatus
    {
        Start,
        Pause,
    }

    #endregion
}

