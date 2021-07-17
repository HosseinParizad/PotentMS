using PotentHelper;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RepeatManager
{
    internal class Engine
    {
        public static void RegisterRepeat(string key, string content)
        {
            var data = JsonSerializer.Deserialize<dynamic>(content);
            var name = data.GetProperty("ReferenceName").ToString();
            var id = data.GetProperty("ReferenceId").ToString();
            var days = int.Parse(data.GetProperty("Days").ToString());

            Repeat.Add(new RepeatItem
            {
                Id = Guid.NewGuid().ToString(),
                ReferenceName = name,
                ReferenceId = id,
                Days = days,
                StartTime = DateTimeOffset.Now,
                LastGeneratedTime = DateTimeOffset.Now
            });
        }

        internal static void Reset(string a, string b) => Repeat = new List<RepeatItem>();

        public static List<RepeatItem> Repeat = new List<RepeatItem>();

        public class RepeatItem
        {
            public string Id { get; set; }
            public string ReferenceName { get; set; }
            public string ReferenceId { get; set; }
            public int Days { get; set; }
            public DateTimeOffset StartTime { get; set; }
            public DateTimeOffset EndTime { get; set; }
            public DateTimeOffset LastGeneratedTime { get; set; }
        }
    }
}