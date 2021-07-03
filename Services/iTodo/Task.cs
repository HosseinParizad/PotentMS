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

        public static void CreateNewTask(string groupKey, string content)
        {
            var newItem = new TodoItem();
            var data = JsonSerializer.Deserialize<dynamic>(content);
            newItem.Id = Guid.NewGuid().ToString();
            newItem.Description = data.GetProperty("Description").ToString();
            var parentId = data.GetProperty("ParentId").ToString();
            if (!string.IsNullOrEmpty(parentId))
            {
                newItem.ParentId = parentId;
            }
            newItem.GroupKey = groupKey;
            newItem.Sequence = Todos.Count;
            Todos.Add(newItem);
            CreateGroupIfNotExists(groupKey);
            var dataToSend = JsonSerializer.Serialize(new { Id = newItem.Id, Text = newItem.Description, ParentId = parentId });
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewTaskAdded, key: groupKey, content: dataToSend);
        }

        public static void CreateNewGoal(string groupKey, string content)
        {
            var newItem = new TodoItem();
            var data = JsonSerializer.Deserialize<dynamic>(content);
            newItem.Id = Guid.NewGuid().ToString();
            newItem.Description = data.GetProperty("Description").ToString();
            newItem.GroupKey = groupKey;
            newItem.Sequence = Todos.Count;
            newItem.Kind = TodoType.Goal;
            Todos.Add(newItem);
            var dataToSend = JsonSerializer.Serialize(new { Id = newItem.Id, Goal = newItem.Description });
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewGoalAdded, key: groupKey, content: dataToSend);
            CreateGroupIfNotExists(groupKey);
        }

        #endregion

        #region UpdateDescription 

        public static void UpdateDescription(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var description = data.GetProperty("Description").ToString();
            FindById(groupKey, id).Description = description;
            var dataToSend = JsonSerializer.Serialize(new { Id = id, Description = description });
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.updateTaskDescription, key: groupKey, content: dataToSend);
        }

        #endregion

        #region SetDeadline 

        public static void SetDeadline(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var deadline = data.GetProperty("Deadline").GetDateTimeOffset();
            TodoItem todo = FindById(groupKey, id);
            todo.Deadline = deadline;
            var dataToSend = JsonSerializer.Serialize(new { Id = id, Text = todo.Description, Deadline = deadline });
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.DeadlineUpdated, key: groupKey, content: dataToSend);
        }

        #endregion

        #region SetTag 

        public static void SetTag(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var tag = data.GetProperty("Tag").ToString();
            var tagKey = data.GetProperty("TagKey").ToString();
            var todo = FindById(groupKey, id);
            if (todo == null)
            {
                SendFeedbackMessage(type: FeedbackType.Error, action: FeedbackActions.CannotSetTag, key: groupKey, content: "Cannot find Todo item to assgin tag!");
            }
            else
            {
                Console.WriteLine(tag);
                UpdateTags(todo, groupKey, tag, tagKey);
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
                    SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewTagAdded, key: groupKey, content: tag);
                }
            }
        }


        #endregion

        #region CloseTask 

        public static void CloseTask(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var task = FindById(groupKey, id);
            if (task != null)
            {
                task.Status = TodoStatus.Close;
                //SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: id, content: null, originalRequest: "CloseTask");
                SendFeedbackMessage(type: FeedbackType.Error, action: FeedbackActions.CannotCloseTask, key: groupKey, content: "Cannot find Todo item to close task!");
            }
            else
            {
                //SendFeedbackMessage(type: FeedbackType.Error, groupKey: groupKey, id: id, content: "Cannot find task!", originalRequest: "CloseTask");
            }
        }

        #endregion

        #region AssignTask

        public static void AssignTask(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var assignTo = data.GetProperty("AssignTo").ToString();
            TodoItem task = FindById(groupKey, id);
            if (task != null)
            {
                task.AssignedTo = assignTo;
            }
            var dataToSend = JsonSerializer.Serialize(new { Id = id, MemberKey = assignTo });
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.TaskAssginedToMember, key: groupKey, content: dataToSend);
        }

        #endregion

        #region SetLocation 

        public static void SetLocation(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            string location = data.GetProperty("Location").ToString();
            TodoItem todo = FindById(groupKey, id);
            if (todo == null)
            {
                //SendFeedbackMessage(type: FeedbackType.Error, groupKey: groupKey, id: id, content: "Cannot find!", originalRequest: "SetLocation");
            }
            else
            {
                todo.Locations.AddRange(location.Split(","));
                var dataToSend = JsonSerializer.Serialize(new { Id = id, Location = location });
                SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewLocationAdded, key: groupKey, content: dataToSend);

            }
        }

        #endregion

        #region NewGroup 

        public static void NewGroup(string groupKey, string content)
        {
            Groups.Add(CreateNewGroup(groupKey, groupKey));
        }

        //static GroupItem CreateNewGroup(string groupKey, string member) => new GroupItem { Group = groupKey, Member = member, Tags = new List<TagSetting>() };

        static GroupItem CreateNewGroup(string groupKey, string member)
        {
            var dataToSend = JsonSerializer.Serialize(new { GroupKey = groupKey, MemberKey = member });
            SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewGroupAdded, key: groupKey, content: dataToSend);
            return new GroupItem { Group = groupKey, Member = member, Tags = new List<TagSetting>() };
        }

        #endregion

        #region NewMember 

        public static void NewMember(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var newMember = data.GetProperty("NewMember").ToString();
            CreateGroupIfNotExists(newMember);
            Groups.Add(CreateNewGroup(groupKey, newMember));
        }

        static void CreateGroupIfNotExists(dynamic groupKey)
        {
            if (!Groups.Any(g => g.Group == groupKey))
            {
                Groups.Add(CreateNewGroup(groupKey, groupKey));
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

        static void SendFeedbackMessage(FeedbackType type, string action, string key, string content)
            => ProducerHelper.SendAMessage(MessageTopic.TaskFeedback, JsonSerializer.Serialize(new Feedback(type: type, name: FeedbackGroupNames.Task, action: action, key: key, content: content))).GetAwaiter().GetResult();

        static TodoItem FindById(string groupKey, dynamic id) => Todos.SingleOrDefault(t => t.GroupKey == groupKey && t.Id == id);

        static List<TodoItem> Todos = new List<TodoItem>();

        static List<GroupItem> Groups { get; set; } = new List<GroupItem>();

        static Dictionary<string, string> MemberCurrentLocation { get; set; } = new Dictionary<string, string>();

        public static string GetSort => Sort;
        static string Sort = "";

        #endregion

        #region Location actions 

        public static void SetCurrentLocation(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var member = data.GetProperty("Member").ToString();
            string location = data.GetProperty("Location").ToString();
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

        public static void Reset(string groupKey, string content)
        {
            Todos = new List<TodoItem>();
            Groups = new List<GroupItem>();
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
        public List<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
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
        Goal,
        Category,
        Task
    }

    #endregion
}

