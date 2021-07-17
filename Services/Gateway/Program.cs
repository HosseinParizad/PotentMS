using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gateway.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PotentHelper;

namespace Gateway
{
    public class Program
    {
        const string AppGroupId = "Gateway";
        static string AppId = AppGroupId + (KafkaEnviroment.preFix == "" ? "" : Guid.NewGuid().ToString());

        public static void Main(string[] args)
        {
            var commonActions =
                new Dictionary<string, Action<string, string>> {
                    { "reset", GatewayController.Reset },
                };


            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.TaskFeedback, (m) => MessageProcessor.MapFeedbackToAction(AppId, m, actions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Common, (m) => MessageProcessor.MapMessageToAction(AppId, m, commonActions), AppId)
                );
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static Dictionary<string, Action<Feedback>> actions =
            new Dictionary<string, Action<Feedback>> {
                { FeedbackGroupNames.Task, GatewayController.MessageReceived },
    };

    }
}
