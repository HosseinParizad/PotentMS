using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PotentHelper;

namespace iTodo
{
    #region Mapping 

    #endregion

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
        }

        #endregion

        #region UpdateDescription 

        public static void UpdateDescription(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var description = data.GetProperty("Description").ToString();
            FindById(groupKey, id).Description = description;
        }

        #endregion

        #region SetDeadline 


        public static void SetDeadline(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var deadline = data.GetProperty("Deadline").GetDateTimeOffset();
            FindById(groupKey, id).Deadline = deadline;
        }


        #endregion

        #region SetCurrentLocation 

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
                UpdateTags(todo, tag, tagKey);
                SendFeedbackMessage(type: FeedbackType.Success, action: FeedbackActions.NewTagAdded, key: groupKey, content: tag);
            }
        }

        #endregion

        #region UpdateTags 

        static void UpdateTags(TodoItem todo, string allTag, string tagKey)
        {
            foreach (var tag in allTag.Split(",").Distinct())
            {
                var found = false;
                foreach (var tagItem in todo.Tags)
                {
                    if (tagItem.TagParentKey == tagKey)
                    {
                        if (("," + tagItem.Value).IndexOf("," + tag) < 0)
                        {
                            tagItem.Value += "," + tag;
                        }
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    todo.Tags.Add(new TagItem { TagParentKey = tagKey, Value = tag });
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
                //SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: id, content: content, originalRequest: "SetLocation");
            }
        }

        #endregion

        #region NewGroup 

        public static void NewGroup(string groupKey, string content)
        {
            Groups.Add(CreateNewGroup(groupKey, groupKey));
            //SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: null, content: null, originalRequest: "NewGroup");
        }

        static GroupItem CreateNewGroup(string groupKey, string member) => new GroupItem { Group = groupKey, Member = member, Tags = new List<TagSetting>() };


        #endregion

        #region NewMember 

        public static void NewMember(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var newMember = data.GetProperty("NewMember").ToString();
            CreateGroupIfNotExists(newMember);
            Groups.Add(CreateNewGroup(groupKey, newMember));
            //SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: null, content: null, originalRequest: "NewMember");
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

        #region GetTask 

        public static IEnumerable<GroupItem> GetGroup(string groupKey)
        {
            if (groupKey == "All")
            {
                return Groups;
            }

            return Groups.Where(i => i.Group == groupKey);
        }


        #endregion

        #region Reset 

        public static void Reset()
        {
            Todos = new List<TodoItem>();
            Groups = new List<GroupItem>();
            MemberCurrentLocation = new Dictionary<string, string>();
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
    }

    #region Classes

    public class TodoItem
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string GroupKey { get; set; }
        public string AssignedTo { get; set; }
        public List<string> Category { get; set; }
        public DateTimeOffset? Deadline { get; set; }
        public int Sequence { get; set; }
        public List<string> Locations { get; set; } = new List<string>();
        public List<TagItem> Tags { get; set; } = new List<TagItem>();
        public TodoStatus Status { get; set; }
        public string ParentId { get; set; }
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
        public string Value { get; set; }
    }

    public enum TodoStatus
    {
        Active,
        Close
    }

    #endregion
}

