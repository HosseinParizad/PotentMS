using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using PotentHelper;

namespace iTodo
{
    public class TodoItem
    {
        public TodoItem(string content, string groupKey)
        {
            Id = Guid.NewGuid().ToString();
            Helper.CastContent(this, content);
            GroupKey = groupKey;
        }

        #region properties

        public string Id { get; }
        public string Description { get; set; }
        public string GroupKey { get; set; }
        public string AssignedTo { get; set; }
        public List<string> Category { get; set; }
        public DateTime? Deadline { get; set; }
        public int Sequence { get; set; }

        #endregion

    }

    public class Helper
    {
        public static Dictionary<string, Action<string, string>> TaskAction =>
        new Dictionary<string, Action<string, string>> {
            { "newTask", Engine.CreateNewTask },
            { "updateTask", Engine.UpdateTask },
         };

        public static void CastContent(TodoItem todoItem, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            todoItem.Description = data.GetProperty("Description").ToString();
        }
    }

    internal class Engine
    {
        public static void CreateNewTask(string groupKey, string content)
        {
            var newItem = new TodoItem(content, groupKey);
            newItem.Sequence = Todos.Count;
            Todos.Add(newItem);
            //Console.WriteLine($"you rech me {groupKey} , {content}  -__*******************************__{string.Join(", -> ", Todos.Select(t => t.Description))}");
            var task = ProducerHelper.SendAMessage("taskCreated", "");

            task.GetAwaiter().GetResult();
        }
        public static void UpdateTask(string groupKey, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var id = data.GetProperty("Id").ToString();
            Helper.CastContent(Todos.Single(t => t.GroupKey == groupKey && t.Id == id), content);
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

        static List<TodoItem> Todos = new List<TodoItem>();
    }
}
