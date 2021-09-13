using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using PotentHelper;

namespace iTodo
{
    public class Engine
    {
        #region CreateNewTask 

        public static void CreateNewTask(dynamic metadata, dynamic content)
        {
            var description = content.Description.ToString();
            var parentId = content.ParentId.ToString();
            var id = metadata.ReferenceKey.ToString();
            if (!Todos.Any(t => (t.Id == id || (t.ParentId == parentId && t.Description == description) && t.GroupKey == metadata.GroupKey.ToString())))
            {
                AddTask(id, metadata.GroupKey.ToString(), description, parentId, GetCreateDate(metadata), TodoType.Task);
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotAddTask, groupkey: metadata.GroupKey.ToString(), content: "Cannot add task id or description are duplicated!");
            }
        }

        static void AddTask(string id, string groupKey, string description, string parentId, DateTimeOffset actionTime, TodoType todoType)
        {
            var newItem = new TodoItem();
            newItem.Id = id;
            newItem.GroupKey = groupKey;
            newItem.Sequence = Todos.Count;
            newItem.Description = description;
            newItem.Kind = todoType;
            newItem.ParentId = parentId ?? "";
            AddTask(newItem, actionTime);
        }

        static void AddTask(TodoItem newItem, DateTimeOffset actionTime)
        {
            Todos.Add(newItem);

            string feedbackActions;
            switch (newItem.Kind)
            {
                case TodoType.Goal:
                    feedbackActions = FeedbackActions.NewGoalAdded;
                    break;
                case TodoType.Category:
                    throw new NotImplementedException();
                case TodoType.Task:
                    feedbackActions = FeedbackActions.NewTaskAdded;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var dataToSend = new { Id = newItem.Id, Text = newItem.Description, ParentId = newItem.ParentId };
            SendFeedbackMessage(type: MsgType.Success, actionTime: actionTime, action: feedbackActions, groupkey: newItem.GroupKey, content: dataToSend);
        }

        #endregion

        #region UpdateDescription 

        public static void UpdateDescription(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var description = content.Description.ToString();
            FindById(metadata.GroupKey.ToString(), id).Description = description;
            var dataToSend = new { Id = id, Description = description };
            SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.updateTaskDescription, groupkey: metadata.GroupKey.ToString(), content: dataToSend);
        }

        #endregion

        #region MoveTask 

        public static void MoveTask(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var toid = content.ToParentId.ToString();
            FindById(metadata.GroupKey.ToString(), id).ParentId = toid;
            var dataToSend = new { Id = id, NewParentId = toid };
            SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.moveTask, groupkey: metadata.GroupKey.ToString(), content: dataToSend);
        }

        #endregion

        #region SetDeadline 

        public static void SetDeadline(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var deadline = DateTimeOffset.Parse(content.Deadline.ToString(), null, DateTimeStyles.AdjustToUniversal);
            TodoItem todo = FindById(metadata.GroupKey.ToString(), id);
            todo.Deadline = deadline;
            var dataToSend = new { Id = id, Text = todo.Description, Deadline = deadline };

            SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.DeadlineUpdated, groupkey: metadata.GroupKey.ToString(), content: dataToSend);
        }

        #endregion

        #region SetTag 

        public static void SetTag(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var tag = content.Tag.ToString();
            var tagKey = content.TagKey.ToString();
            var todo = FindById(metadata.GroupKey.ToString(), id);
            if (todo == null)
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotSetTag, groupkey: metadata.GroupKey.ToString(), content: "Cannot find Todo item to assgin tag!");
            }
            else
            {
                Console.WriteLine(tag);
                UpdateTags(todo, metadata.GroupKey.ToString(), tag, tagKey, GetCreateDate(metadata));
            }
        }

        #endregion

        #region UpdateTags

        static void UpdateTags(TodoItem todo, string groupKey, string allTag, string tagKey, DateTimeOffset actionTime)
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
                    SendFeedbackMessage(type: MsgType.Success, actionTime: actionTime, action: FeedbackActions.NewTagAdded, groupkey: groupKey, content: tag);
                }
            }
        }

        #endregion

        #region CloseTask 

        public static void CloseTask(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var task = FindById(metadata.GroupKey.ToString(), id);
            if (task != null)
            {
                task.Status = TodoStatus.Close;
                var dataToSend = new { Id = id };
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.TaskClosed, groupkey: metadata.GroupKey.ToString(), content: dataToSend);
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: FeedbackActions.CannotCloseTask, groupkey: metadata.GroupKey.ToString(), content: "Cannot find Todo item to close task!");
            }
        }

        #endregion

        #region DeleteTask 

        public static void DeleteTask(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var task = FindById(metadata.GroupKey.ToString(), id);
            if (task != null)
            {
                Todos.Remove(task);

                var dataToSend = new { Id = id };
                if (task.Kind == TodoType.Task)
                {
                    SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.TaskDeleted, groupkey: metadata.GroupKey.ToString(), content: dataToSend);
                }
                else if (task.Kind == TodoType.Goal)
                {
                    SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.GoalDeleted, groupkey: metadata.GroupKey.ToString(), content: dataToSend);
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
            var id = content.Id.ToString();
            var assignTo = content.AssignTo.ToString();
            TodoItem task = FindById(metadata.GroupKey.ToString(), id);
            if (task != null)
            {
                task.AssignedTo = assignTo;
            }
            var dataToSend = new { Id = id, MemberKey = assignTo };
            SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.TaskAssginedToMember, groupkey: metadata.GroupKey.ToString(), content: dataToSend);
        }

        #endregion

        #region SetLocation 

        public static void SetLocation(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            string location = content.Location.ToString();
            TodoItem todo = FindById(metadata.GroupKey.ToString(), id);
            if (todo == null)
            {
                //SendFeedbackMessage(type: FeedbackType.Error, groupKey: groupKey, id: id, content: "Cannot find!", originalRequest: "SetLocation");
            }
            else
            {
                var locations = location.Split(",");
                todo.Locations.AddRange(locations);
                foreach (var item in locations)
                {
                    var dataToSend = new { Id = id, Location = item };

                    SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: FeedbackActions.NewLocationAdded, groupkey: metadata.GroupKey.ToString(), content: dataToSend);
                }
            }
        }

        #endregion

        #region RepeatTask

        public static void RepeatTask(Feedback feedback)
        {
            var id = feedback.Content.Id.ToString();
            var repeatIfAllClosed = bool.Parse(feedback.Content.RepeatIfAllClosed?.ToString() ?? "false");
            var date = DateTimeOffset.Parse(feedback.Content.LastGeneratedTime.ToString());
            var hours = int.Parse(feedback.Content.Hours.ToString());
            var dateStr = " (" + date.Date.ToShortDateString() + ")";

            TodoItem task = Todos.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                var shouldRepeat = !repeatIfAllClosed || !Todos.Where(t => (t.OriginalRepeatId == id || t.Id == id) && t.Status != TodoStatus.Close).Any();
                if (shouldRepeat)
                {
                    AddTaskAndChildrenRepeat(task, task.ParentId, dateStr, hours, id, actionTime: GetCreateDate(feedback.Metadata));
                }
            }
        }

        static void AddTaskAndChildrenRepeat(TodoItem task, string parentId, string date, int hours, string originalRepeatId, DateTimeOffset actionTime)
        {
            var ctask = task.Clone();
            ctask.ParentId = parentId;
            ctask.Description += date;
            ctask.Deadline = task.Deadline?.AddHours(hours);
            ctask.Status = TodoStatus.Active;
            ctask.OriginalRepeatId = originalRepeatId;
            AddTask(ctask, actionTime: actionTime);
            foreach (var child in Todos.Where(t => t.ParentId == task.Id).ToArray())
            {
                AddTaskAndChildrenRepeat(child, ctask.Id, date, hours, originalRepeatId, actionTime);
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

        static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, string groupkey, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                var feedback = new Feedback(type: type, action: action, metadata: Helper.GetMetadataByGroupKey(groupkey), content: content);
                ProducerHelper.SendAMessage(MessageTopic.TaskFeedback, feedback).GetAwaiter().GetResult();
            }
        }

        static TodoItem FindById(string groupKey, dynamic id) => Todos.SingleOrDefault(t => t.GroupKey == groupKey && t.Id == id);
        static TodoItem FindFirstByCondition(string groupKey, Func<TodoItem, bool> condition) => Todos.FirstOrDefault(t => t.GroupKey == groupKey && condition(t));

        static List<TodoItem> Todos = new List<TodoItem>();
        static List<TimeItem> TimeLog = new List<TimeItem>();

        static Dictionary<string, string> MemberCurrentLocation { get; set; } = new Dictionary<string, string>();

        public static string GetSort => Sort;
        static string Sort = "";

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        #endregion

        #region Location actions 

        public static void SetCurrentLocation(dynamic metadata, dynamic content)
        {
            var member = content.Member.ToString();
            string location = content.Location.ToString();
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
            TimeLog = new List<TimeItem>();
            MemberCurrentLocation = new Dictionary<string, string>();
        }

        #endregion

        #region GetPresentation

        internal static IEnumerable<PresentItem> GetPresentationTask(string groupKey, string parentId)
        {
            return Todos.Where(i => i.Kind != TodoType.Goal && i.Status != TodoStatus.Close && (i.AssignedTo ?? i.GroupKey) == groupKey && i.ParentId == parentId)
                .Select(i => GetPresentationItemTask(groupKey, i));
        }

        static PresentItem GetPresentationItemTask(string groupKey, TodoItem todo)
        {
            var presentItem = new PresentItem
            {
                Id = todo.Id,
                Text = todo.Description,
                Link = todo.Kind.ToString(),
                Actions = GetActions(todo).ToList(),
                Items = GetPresentationTask(groupKey, todo.Id).ToList()
            };
            return presentItem;
        }

        static IEnumerable<PresentItemActions> GetActions(TodoItem todo)
        {
            Func<string, string, dynamic, PresentItemActions> createStep = (text, action, content) =>
               new PresentItemActions
               {
                   Text = text,
                   Group = "",
                   Action = action,
                   Metadata = new { GroupKey = todo.GroupKey, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };

            yield return createStep("step", MapAction.Task.NewTask, new { Description = "[text]", ParentId = todo.Id });
            yield return createStep("update", MapAction.Task.UpdateDescription, new { Description = "[text]", Id = todo.Id });
            yield return createStep("delete", MapAction.Task.DelTask, new { Id = todo.Id });
            yield return createStep("close", MapAction.Task.CloseTask, new { Id = todo.Id });
            yield return createStep("location", MapAction.Task.SetLocation, new { Location = "[text]", Id = todo.Id });
            yield return createStep("tag", MapAction.Task.SetTag, new { Tag = "[text]", TagKey = 0, Id = todo.Id });
            yield return createStep("close", MapAction.Task.CloseTask, new { Id = todo.Id });
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
        public string OriginalRepeatId { get; set; }

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
        Close
    }

    public enum TodoType
    {
        Category,
        Task,
        Goal,
    }

    public enum TimeActionStatus
    {
        Start,
        Pause,
    }

    #endregion
}

