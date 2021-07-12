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
        static string AppId = AppGroupId + (KafkaEnviroment.preFix == "" ? "" : Guid.NewGuid().ToString());

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

            var locationActions =
                new Dictionary<string, Action<string, string>> {
                    { "setCurrentLocation", Engine.SetCurrentLocation },
                };

            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.TaskFeedback, (m) => MessageProcessor.MapFeedbackToAction(m, taskFeedbackActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Common, (m) => MessageProcessor.MapMessageToAction(m, commonActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Location, (m) => MessageProcessor.MapMessageToAction(m, locationActions), AppId)
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
