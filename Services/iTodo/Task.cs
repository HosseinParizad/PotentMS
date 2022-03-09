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
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            if (!Todos.Any(t => (t.Id == id || (t.ParentId == parentId && t.Description == description) && t.GroupKey == groupKey && t.MemberKey == memberKey)))
            {
                AddTask(id, groupKey, memberKey, description, parentId, GetCreateDate(metadata), TodoType.Task);
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.CannotAddTask.Name, content: "Cannot add task id or description are duplicated!");
            }
        }

        static void AddTask(string id, string groupKey, string memberKey, string description, string parentId, DateTimeOffset actionTime, TodoType todoType)
        {
            var newItem = new TodoItem
            {
                Id = id,
                GroupKey = groupKey,
                MemberKey = memberKey,
                Sequence = Todos.Count,
                Description = description,
                Kind = todoType,
                ParentId = parentId ?? ""
            };
            AddTask(newItem, actionTime);
        }

        static void AddTask(TodoItem newItem, DateTimeOffset actionTime)
        {
            Todos.Add(newItem);

            string feedbackActions;
            switch (newItem.Kind)
            {
                case TodoType.Category:
                    throw new NotImplementedException();
                case TodoType.Task:
                    feedbackActions = MapAction.TaskFeedback.NewTaskAdded.Name;
                    break;
                default:
                    throw new NotImplementedException();
            }

            var dataToSend = new { GroupKey = newItem.GroupKey, MemberKey = newItem.MemberKey, Id = newItem.Id, Text = newItem.Description, ParentId = newItem.ParentId };
            SendFeedbackMessage(type: MsgType.Success, actionTime: actionTime, action: feedbackActions, content: dataToSend);
        }

        #endregion

        #region UpdateDescription 

        public static void UpdateDescription(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var description = content.Description.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            FindById(metadata.GroupKey.ToString(), id).Description = description;
            var dataToSend = new { GroupKey = groupKey, MemberKey = memberKey, Id = id, Description = description };
            SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.updateTaskDescription.Name, content: dataToSend);
        }

        #endregion

        #region MoveTask 

        public static void MoveTask(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var toid = content.ToParentId.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            var task = Todos.SingleOrDefault(t => t.Id == id);
            var newParent = Todos.SingleOrDefault(t => t.Id == toid);
            if (task != null && newParent != null)
            {
                task.GroupKey = newParent.GroupKey;
                task.MemberKey = newParent.MemberKey;
                task.ParentId = toid;
                var dataToSend = new { GroupKey = groupKey, MemberKey = memberKey, Id = id, NewParentId = toid };
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.Task.MoveTask.Name, content: dataToSend);
            }
        }

        #endregion

        #region SetDeadline 

        public static void SetDeadline(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            var deadline = DateTimeOffset.Parse(content.Deadline.ToString(), null, DateTimeStyles.AdjustToUniversal);
            TodoItem todo = FindById(groupKey, id);
            todo.Deadline = deadline;
            var dataToSend = new { GroupKey = groupKey, MemberKey = memberKey, Id = id, Text = todo.Description, Deadline = deadline };
            SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.DeadlineUpdated.Name, content: dataToSend);
        }

        #endregion

        #region SetTag 

        public static void SetTag(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var tag = content.Tag.ToString();
            var tagKey = content.TagKey.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            var todo = FindById(metadata.GroupKey.ToString(), id);
            if (todo == null)
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.CannotSetTag.Name, content: "Cannot find Todo item to assgin tag!");
            }
            else
            {
                Console.WriteLine(tag);
                UpdateTags(todo, groupKey, memberKey, tag, tagKey, GetCreateDate(metadata));
            }
        }

        #endregion

        #region UpdateTags

        static void UpdateTags(TodoItem todo, string groupKey, string memberKey, string allTag, string tagKey, DateTimeOffset actionTime)
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
                    SendFeedbackMessage(type: MsgType.Success, actionTime: actionTime, action: MapAction.TaskFeedback.NewTagAdded.Name, content: tag);
                }
            }
        }

        #endregion

        #region CloseTask 

        public static void CloseTask(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            var task = FindById(metadata.GroupKey.ToString(), id);
            if (task != null)
            {
                task.Status = TodoStatus.Close;
                var dataToSend = new { GroupKey = groupKey, MemberKey = memberKey, Id = id };
                SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.TaskClosed.Name, content: dataToSend);
            }
            else
            {
                SendFeedbackMessage(type: MsgType.Error, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.CannotCloseTask.Name, content: "Cannot find Todo item to close task!");

            }
        }

        #endregion

        #region DeleteTask 

        public static void DeleteTask(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            var task = FindById(metadata.GroupKey.ToString(), id);
            if (task != null)
            {
                Todos.Remove(task);

                var dataToSend = new { GroupKey = groupKey, MemberKey = memberKey, Id = id };
                if (task.Kind == TodoType.Task)
                {
                    SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.TaskDeleted.Name, content: dataToSend);
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
            var groupKey = metadata.GroupKey.ToString();
            var assignTo = content.AssignTo.ToString();
            TodoItem task = FindById(groupKey, id);
            if (task != null)
            {
                task.MemberKey = assignTo;
            }
            var dataToSend = new { GroupKey = groupKey, MemberKey = assignTo, Id = id };
            SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.TaskAssginedToMember.Name, content: dataToSend);
        }

        #endregion

        #region SetLocation 

        public static void SetLocation(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            string location = content.Location.ToString();
            var groupKey = metadata.GroupKey.ToString();
            var memberKey = metadata.MemberKey.ToString();
            TodoItem todo = FindById(groupKey, id);
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
                    var dataToSend = new { GroupKey = groupKey, MemberKey = memberKey, Id = id, Location = item };

                    SendFeedbackMessage(type: MsgType.Success, actionTime: GetCreateDate(metadata), action: MapAction.TaskFeedback.NewLocationAdded.Name, content: dataToSend);
                }
            }
        }

        #endregion

        #region RepeatTask

        public static void RepeatTask(dynamic metadata, dynamic content)
        {
            var id = content.Id.ToString();
            var repeatIfAllClosed = bool.Parse(content.RepeatIfAllClosed?.ToString() ?? "false");
            var date = DateTimeOffset.Parse(content.LastGeneratedTime.ToString());
            var hours = int.Parse(content.Hours.ToString());
            var dateStr = " (" + date.Date.ToShortDateString() + ")";

            TodoItem task = Todos.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                var shouldRepeat = !repeatIfAllClosed || !Todos.Where(t => (t.OriginalRepeatId == id || t.Id == id) && t.Status != TodoStatus.Close).Any();
                if (shouldRepeat)
                {
                    AddTaskAndChildrenRepeat(task, task.ParentId, dateStr, hours, id, actionTime: GetCreateDate(metadata));
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
            return Todos.Where(i => i.Status != TodoStatus.Close && (i.MemberKey ?? i.GroupKey) == member).OrderBy(t => t.Sequence);
        }

        #endregion

        #region GetTaskByGroupTag

        internal static IEnumerable<TodoItem> GetTaskByGroupTag(string groupKey, string tag)
        {
            return Todos.Where(i => i.Status != TodoStatus.Close && (i.MemberKey ?? i.GroupKey) == groupKey && i.Tags.Any(t => t.Value.Contains(tag)));
        }

        #endregion

        #region GetTaskWhenMoveToLocation

        internal static IEnumerable<TodoItem> GetTaskWhenMoveToLocation(string groupKey, string tag)
        {
            return Todos.Where(i => i.Status != TodoStatus.Close && (i.MemberKey ?? i.GroupKey) == groupKey && i.Locations.Any(l => l.IndexOf(tag) > -1));
        }

        #endregion

        #region Implement

        static void SendFeedbackMessage(MsgType type, string action, DateTimeOffset actionTime, dynamic content)
        {
            if (Program.StartingTimeApp < actionTime)
            {
                var feedback = new Feedback(type: type, action: action, content: content);
                ProducerHelper.SendMessage(MessageTopic.TaskFeedback, feedback).GetAwaiter().GetResult();
            }
        }

        static TodoItem FindById(string groupKey, dynamic id) => Todos.SingleOrDefault(t => t.GroupKey == groupKey && t.Id == id);
        static TodoItem FindFirstByCondition(string groupKey, Func<TodoItem, bool> condition) => Todos.FirstOrDefault(t => t.GroupKey == groupKey && condition(t));

        static List<TodoItem> Todos = new();
        static List<TimeItem> TimeLog = new();

        public static string GetSort => Sort;
        static string Sort = "";

        static DateTimeOffset GetCreateDate(dynamic metadata)
        {
            return DateTimeOffset.Parse(metadata.CreateDate.ToString());
        }

        #endregion

        #region Common actions

        public static void Reset(dynamic metadata, dynamic content)
        {
            Todos = new();
            TimeLog = new();
        }

        #endregion

        #region GetPresentation

        internal static IEnumerable<PresentItem> GetPresentationTask(string groupKey, string memberKey, string parentId)
        {
            return Todos.Where(i => i.Kind != TodoType.Goal && i.Status != TodoStatus.Close).GetGroupMember(groupKey, memberKey, parentId)
                .Select(i => GetPresentationItemTask(groupKey, memberKey, i));
        }

        static PresentItem GetPresentationItemTask(string groupKey, string memberKey, TodoItem todo)
        {
            var presentItem = new PresentItem
            {
                Id = todo.Id,
                Text = todo.Description,
                Link = todo.Kind.ToString(),
                Actions = GetActions(todo).ToList(),
                Items = GetPresentationTask(groupKey, memberKey, todo.Id).ToList(),
                Info = GetInfo(todo)
            };
            return presentItem;
        }

        static string GetInfo(TodoItem todo)
        {
            return (todo.Locations.Any() ? $"Locations: {string.Join("|", todo.Locations)}" : "") +
                (todo.Tags.Any() ? $"Tags: {string.Join("|", todo.Tags.Select(t => $"{t.TagParentKey}-{string.Join('&', t.Value)}"))}" : "");
        }

        static IEnumerable<PresentItemActions> GetActions(TodoItem todo)
        {
            Func<string, string, dynamic, PresentItemActions> createStep = (text, action, content) =>
               new PresentItemActions
               {
                   Text = text,
                   Group = "",
                   Action = action,
                   Metadata = new { todo.GroupKey, todo.MemberKey, ReferenceKey = Guid.NewGuid().ToString() },
                   Content = content
               };

            yield return createStep("step", MapAction.Task.NewTask.Name, new { Description = "[text]", ParentId = todo.Id });
            yield return createStep("update", MapAction.Task.UpdateDescription.Name, new { Description = "[text]", Id = todo.Id });
            yield return createStep("delete", MapAction.Task.DelTask.Name, new { Id = todo.Id });
            yield return createStep("close", MapAction.Task.CloseTask.Name, new { Id = todo.Id });
            yield return createStep("location", MapAction.Task.SetLocation.Name, new { Location = "[text]", Id = todo.Id });
            yield return createStep("tag", MapAction.Task.SetTag.Name, new { Tag = "[text]", TagKey = 0, Id = todo.Id });
            yield return createStep("close", MapAction.Task.CloseTask.Name, new { Id = todo.Id });
        }

        #endregion
    }

    #region Classes

    public class TodoItem : IMultiGroupParent
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string GroupKey { get; set; }
        public string MemberKey { get; set; }
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

