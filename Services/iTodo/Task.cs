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
            Sort = data.GetProperty("Location").ToString();
            //Console.WriteLine($"you rech me {groupKey} , {content}  -__***************&&&&{Sort}&&&{Todos.Count()}****************__{string.Join(", -> ", Todos.Select(t => t.Description))}");
            SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: null, message: null, originalRequest: "SetCurrentLocation");
        }

        public static void SetTag(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var tag = data.GetProperty("Tag").ToString();
            var todo = FindById(groupKey, id);
            if (todo == null)
            {
                //Console.WriteLine($"you rech me {groupKey} , {content}  -__*******************************__{string.Join(", -> ", Todos.Select(t => t.Tags.First()))}");
                SendFeedbackMessage(type: FeedbackType.Error, groupKey: groupKey, id: id, message: "Cannot find!", originalRequest: "SetTag");
            }
            else
            {
                todo.Tags.AddRange(tag.Split(","));
                //Console.WriteLine($"you rech me {groupKey} , {content}  -__*******************************__{string.Join(", -> ", Todos.Select(t => t.Tags.First()))}");
                SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: id, message: null, originalRequest: "SetTag");
            }
        }

        public static void NewGroup(string groupKey, string content)
        {
            Groups.Add(new GroupItem { Group = groupKey, Member = groupKey });
            SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: null, message: null, originalRequest: "NewGroup");
        }


        public static void NewMember(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var newMember = data.GetProperty("NewMember").ToString();
            CreateGroupIfNotExists(newMember);
            Groups.Add(new GroupItem { Group = groupKey, Member = newMember });
            SendFeedbackMessage(type: FeedbackType.Success, groupKey: groupKey, id: null, message: null, originalRequest: "NewMember");
        }

        static void CreateGroupIfNotExists(dynamic groupKey)
        {
            if (!Groups.Any(g => g.Group == groupKey))
            {
                Groups.Add(new GroupItem { Group = groupKey, Member = groupKey });
            }
        }

        public static IEnumerable<TodoItem> GetTask(string groupKey)
        {
            if (groupKey == "All")
            {
                return Todos;
            }

            return Todos.Where(i => i.GroupKey == groupKey).OrderBy(t => (t.Tags?.Contains(Sort) ?? false) ? 0 : 1).ThenBy(t => t.Sequence);
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
        }

        #region Implement

        static void SendFeedbackMessage(FeedbackType type, string groupKey, string id, string message, string originalRequest) => ProducerHelper.SendAMessage("taskFeedback", JsonSerializer.Serialize(new Feedback(type: type, groupKey: groupKey, id: id, message: message, originalRequest: originalRequest))).GetAwaiter().GetResult();
        static TodoItem FindById(string groupKey, dynamic id) => Todos.SingleOrDefault(t => t.GroupKey == groupKey && t.Id == id);

        static List<TodoItem> Todos = new List<TodoItem>();
        static string Sort = "";

        static List<GroupItem> Groups { get; set; } = new List<GroupItem>();

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
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class GroupItem
    {
        public string Group { get; set; }
        public string Member { get; set; }
    }

    #endregion

}
