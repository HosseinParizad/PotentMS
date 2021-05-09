using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PotentHelper;

namespace iTodo
{
    public class Helper
    {
        public static Dictionary<string, Action<string, string>> TaskAction =>
        new Dictionary<string, Action<string, string>> {
            { "newTask", Engine.CreateNewTask },
            { "updateDescription", Engine.UpdateDescription },
            { "setDeadline", Engine.SetDeadline },
            { "setTag", Engine.SetTag },
            { "setCurrentLocation", Engine.SetCurrentLocation },
            { "newGroup", Engine.NewGroup },
            { "newMember", Engine.NewMember },
            { "setLocation", Engine.SetLocation },
         };

        public static string PropertyString(dynamic obj, string name) => (PropertyExists(obj, name) ? obj.GetProperty(name).ToString() : "");
        public static bool PropertyExists(dynamic obj, string name) => ((IDictionary<string, object>)obj).ContainsKey(name);
    }

    internal class Engine
    {
        public static void CreateNewTask(string groupKey, string content)
        {
            var newItem = new TodoItem();
            var data = JsonSerializer.Deserialize<dynamic>(content);
            newItem.Id = Guid.NewGuid().ToString();
            newItem.Description = data.GetProperty("Description").ToString();
            newItem.GroupKey = groupKey;
            newItem.Sequence = Todos.Count;
            Todos.Add(newItem);
            CreateGroupIfNotExists(groupKey);
        }

        public static void UpdateDescription(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var description = data.GetProperty("Description").ToString();
            FindById(groupKey, id).Description = description;
            SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: id, message: null, originalRequest: "UpdateDescription");
        }

        public static void SetDeadline(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var deadline = data.GetProperty("Deadline").GetDateTimeOffset();
            FindById(groupKey, id).Deadline = deadline;
            SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: id, message: null, originalRequest: "SetDeadline");
        }

        public static void SetCurrentLocation(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var member = data.GetProperty("Member").ToString();
            string location = data.GetProperty("Location").ToString();
            Console.WriteLine($"you rech me {groupKey} , {content}  -__***************&&&&{location}&&&{member}****************__{string.Join(", -> ", Todos.Select(t => t.Locations))}");
            //var locations = string.Empty;
            if (MemeberCurrentLocation.TryGetValue(member, out string locations))
            {
                MemeberCurrentLocation[member] = string.Join(",", locations.Split(",").Union(location.Split(",")).Distinct());
            }
            else
            {
                MemeberCurrentLocation.Add(member, location);
            }
            Console.WriteLine($"you rech me {MemeberCurrentLocation[member]} =============================");

            SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: null, message: null, originalRequest: "SetCurrentLocation");
        }

        public static void SetTag(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var tag = data.GetProperty("Tag").ToString();
            var tagKey = data.GetProperty("TagKey").ToString();
            var todo = FindById(groupKey, id);
            if (todo == null)
            {
                //Console.WriteLine($"you rech me {groupKey} , {content}  -__*******************************__{string.Join(", -> ", Todos.Select(t => t.Tags.First()))}");
                SendFeedbackMessage(type: FeedbackType.Error, groupKey: groupKey, id: id, message: "Cannot find!", originalRequest: "SetTag");
            }
            else
            {
                UpdateTags(todo, tag, tagKey);
                SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: id, message: null, originalRequest: "SetTag");
            }
        }

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

        public static void SetLocation(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var location = data.GetProperty("Location").ToString();
            var todo = FindById(groupKey, id);
            if (todo == null)
            {
                SendFeedbackMessage(type: FeedbackType.Error, groupKey: groupKey, id: id, message: "Cannot find!", originalRequest: "SetLocation");
            }
            else
            {
                todo.Locations.AddRange(location.Split(","));
                SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: id, message: null, originalRequest: "SetLocation");
            }
        }

        public static void NewGroup(string groupKey, string content)
        {
            Groups.Add(CreateNewGroup(groupKey, groupKey));
            SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: null, message: null, originalRequest: "NewGroup");
        }

        static GroupItem CreateNewGroup(string groupKey, string member) => new GroupItem { Group = groupKey, Member = member, Tags = new List<TagSetting>() };

        public static void NewMember(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var newMember = data.GetProperty("NewMember").ToString();
            CreateGroupIfNotExists(newMember);
            Groups.Add(CreateNewGroup(groupKey, newMember));
            SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: null, message: null, originalRequest: "NewMember");
        }

        static void CreateGroupIfNotExists(dynamic groupKey)
        {
            if (!Groups.Any(g => g.Group == groupKey))
            {
                Groups.Add(CreateNewGroup(groupKey, groupKey));
            }
        }

        public static IEnumerable<TodoItem> GetTask(string member)
        {
            if (member == "All")
            {
                return Todos;
            }
            return Todos.Where(i => (i.AssignedTo ?? i.GroupKey) == member).OrderBy(t => MemeberCurrentLocation.ContainsKey(member) && MemeberCurrentLocation[member].Split(",").Any(l => t.Locations?.Contains(l) ?? false) ? 0 : 1).ThenBy(t => t.Sequence);
        }

        public static IEnumerable<GroupItem> GetGroup(string groupKey)
        {
            if (groupKey == "All")
            {
                return Groups;
            }

            return Groups.Where(i => i.Group == groupKey);
        }

        public static string GetSort => Sort;

        public static void Reset()
        {
            Todos = new List<TodoItem>();
            Groups = new List<GroupItem>();
            MemeberCurrentLocation = new Dictionary<string, string>();
        }

        #region Implement

        static void SendFeedbackMessage(FeedbackType type, string groupKey, string id, string message, string originalRequest) => ProducerHelper.SendAMessage("taskFeedback", JsonSerializer.Serialize(new Feedback(type: type, groupKey: groupKey, id: id, message: message, originalRequest: originalRequest))).GetAwaiter().GetResult();
        static TodoItem FindById(string groupKey, dynamic id) => Todos.SingleOrDefault(t => t.GroupKey == groupKey && t.Id == id);

        static List<TodoItem> Todos = new List<TodoItem>();
        static string Sort = "";

        static List<GroupItem> Groups { get; set; } = new List<GroupItem>();

        static Dictionary<string, string> MemeberCurrentLocation { get; set; } = new Dictionary<string, string>();

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

    #endregion

}
