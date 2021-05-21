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
            var taskFeedbackActions =
                new Dictionary<string, Action<Feedback>>
                {
                   { FeedbackGroupNames.Task, Engine.OnTaskFeedback },
                };

            var commonActions =
                new Dictionary<string, Action<string, string>> {
                    { "reset", Engine.Reset },
                };

            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.TaskFeedback, (m) => MessageProcessor.MapFeedbackToAction(m, taskFeedbackActions), AppGroupId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Common, (m) => MessageProcessor.MapMessageToAction(m, commonActions), AppGroupId)
                );
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
