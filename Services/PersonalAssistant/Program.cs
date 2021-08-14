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
            KafkaEnviroment.TempPrefix = args[0];
            var AppId = KafkaEnviroment.preFix + AppGroupId + (KafkaEnviroment.preFix == "" ? "" : Guid.NewGuid().ToString());

            var taskFeedbackActions =
                new Dictionary<string, Action<Feedback>>
                {
                   { FeedbackGroupNames.Task, Engine.OnTaskFeedback },
                };

            var commonActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", Engine.Reset },
                };

            var locationActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "setCurrentLocation", Engine.SetCurrentLocation },
                };

            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.TaskFeedback, (m) => MessageProcessor.MapFeedbackToAction(AppId, m, taskFeedbackActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Common, (m) => MessageProcessor.MapMessageToAction(AppId, m, commonActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Location, (m) => MessageProcessor.MapMessageToAction(AppId, m, locationActions), AppId)
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
