using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using KafkaHelper;

namespace iTodo
{
    public class TodoItem
    {
        public TodoItem(string content, string groupKey)
        {
            Id = Guid.NewGuid().ToString();
            CastContent(content);
            GroupKey = groupKey;
        }

        #region properties

        string Id { get; }
        public string Description { get; set; }
        public string GroupKey { get; set; }
        public string AssignedTo { get; set; }
        public List<string> Category { get; set; }
        public DateTime? Deadline { get; set; }
        public int Sequence { get; set; }

        #endregion

        void CastContent(string content)
        {
            var todoItem = JsonSerializer.Deserialize<dynamic>(content);
            Description = todoItem.GetProperty("Description").ToString();
        }
    }

    public class Helper
    {
        public static Dictionary<string, Action<string, string>> TaskAction =>
        new Dictionary<string, Action<string, string>> {
            { "newTask", Engine.CreateNewTask },
            { "UpdateTask", Engine.UpdateTask },
         };
    }

    internal class Engine
    {
        public static void CreateNewTask(string groupKey, string content)
        {
            var newItem = new TodoItem(content, groupKey);
            newItem.Sequence = Todos.Count;
            Todos.Add(newItem);
            Console.WriteLine($"you rech me {groupKey} , {content}");
            var task = Producer.SendAMessage("taskCreated", "");
            task.GetAwaiter().GetResult();
        }
        public static void UpdateTask(string belongTo, string content)
        {

        }

        public static IEnumerable<TodoItem> GetTask(string groupKey)
        {
            if (groupKey == "All")
            {
                return Todos.OrderBy(t => t.Sequence);
            }

            return Todos.Where(i => i.GroupKey == groupKey).OrderBy(t => t.Sequence);
        }

        static List<TodoItem> Todos = new List<TodoItem>();
    }
}
