using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PotentHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace PersonalAssistant
{
    public class Program
    {
        const string AppGroupId = "PersonalAssistant";

        public static void Main(string[] args)
        {
            Parallel.Invoke(
                () => CreateHostBuilder(args).Build().Run(),
                //ConsumerHelper.MapTopicToMethod("task", (m) => MessageProcessor.MapMessageToAction(m, actions), AppGroupId),
                //ConsumerHelper.MapTopicToMethod("location", (m) => MessageProcessor.MapMessageToAction(m, actions), AppGroupId),
                ConsumerHelper.MapTopicToMethod(MessageTopic.TaskFeedback, (m) => MessageProcessor.MapFeedbackToAction(m, actions), AppGroupId)
            );
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static Dictionary<string, Action<Feedback>> actions =
            new Dictionary<string, Action<Feedback>>
            {
                { FeedbackGroupNames.Task, Engine.OnTaskFeedback },
            };

    }
}
