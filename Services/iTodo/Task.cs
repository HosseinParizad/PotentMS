using System.IO;
using System;
using System.Collections.Generic;

namespace iTodo
{
    internal class Task
    {
        public Task(string description, string groupKey)
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

    class Engine
    {
        public static void CreateNewTask(string belongTo, string content)
        {
            Console.WriteLine($"you rech me {belongTo} , {content}");
        }
        public static void UpdateTask(string belongTo, string content)
        {

        }
    }
}
