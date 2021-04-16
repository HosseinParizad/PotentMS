using System;
using System.Collections.Generic;
using System.Linq;
using KafkaHelper;

namespace iTodo
{
    public class TodoItem
    {
        public TodoItem(string description, string groupKey)
        {
            Id = Guid.NewGuid().ToString();
            Description = description;
            GroupKey = groupKey;
        }

        #region properties

        string Id { get; }
        public string Description { get; set; }
        public string GroupKey { get; set; }
        public string AssignedTo { get; set; }
        public List<string> Category { get; set; }
        public DateTime? Deadline { get; set; }

        #endregion
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
            Todos.Add(new TodoItem(content, groupKey));
            Console.WriteLine($"you rech me {groupKey} , {content}");
            var task = Producer.SendAMessage("taskCreated", "");
            task.GetAwaiter().GetResult();
        }
        public static void UpdateTask(string belongTo, string content)
        {

        }

        public static IEnumerable<TodoItem> GetTask(string groupKey)
        {
            return Todos.Where(i => i.GroupKey == groupKey);
        }

        static List<TodoItem> Todos = new List<TodoItem>();
    }
}
