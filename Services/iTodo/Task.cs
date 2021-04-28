using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PotentHelper;

namespace iTodo
{
    public class TodoItem
    {
        //public TodoItem(string content, string groupKey)
        //{
        //    Id = Guid.NewGuid().ToString();
        //    Helper.CastContent(this, content);
        //    GroupKey = groupKey;
        //}

        #region properties

        public string Id { get; set; }
        public string Description { get; set; }
        public string GroupKey { get; set; }
        public string AssignedTo { get; set; }
        public List<string> Category { get; set; }
        public DateTimeOffset? Deadline { get; set; }
        public int Sequence { get; set; }
        public List<string> Tags { get; set; } = new List<string>();

        #endregion

    }

    public class Helper
    {
        public static Dictionary<string, Action<string, string>> TaskAction =>
        new Dictionary<string, Action<string, string>> {
            { "newTask", Engine.CreateNewTask },
            { "updateDescription", Engine.UpdateDescription },
            { "setDeadline", Engine.SetDeadline },
            { "setTag", Engine.SetTag },
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
            //Console.WriteLine($"you rech me {groupKey} , {content}  -__*******************************__{string.Join(", -> ", Todos.Select(t => t.Description))}");
            SendDoneMessage("taskCreated", "");
        }

        public static void UpdateDescription(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var description = data.GetProperty("Description").ToString();
            FindById(groupKey, id).Description = description;
            SendDoneMessage("taskDescriptionUpdated", "");
        }

        public static void SetDeadline(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var deadline = data.GetProperty("Deadline").GetDateTimeOffset();
            FindById(groupKey, id).Deadline = deadline;
            SendDoneMessage("taskDeadlineHasBeenSet", "");
        }

        public static void SetTag(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            var tag = data.GetProperty("Tag").ToString();
            FindById(groupKey, id).Tags.AddRange(tag.Split(","));
            SendDoneMessage("taskTagHasBeenSet", "");
        }

        public static IEnumerable<TodoItem> GetTask(string groupKey)
        {
            if (groupKey == "All")
            {
                return Todos;
            }

            return Todos.Where(i => i.GroupKey == groupKey).OrderBy(t => t.Sequence);
        }

        public static void Reset()
        {
            Todos = new List<TodoItem>();
        }

        static void SendDoneMessage(string sendMessageTopic, string msg) => ProducerHelper.SendAMessage(sendMessageTopic, msg).GetAwaiter().GetResult();
        static TodoItem FindById(string groupKey, dynamic id) => Todos.Single(t => t.GroupKey == groupKey && t.Id == id);

        static List<TodoItem> Todos = new List<TodoItem>();
    }
}
