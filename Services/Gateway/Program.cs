using System;
using System.Collections.Generic;
using System.Threading;
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

        public static void Main(string[] args)
        {
            var commonActions =
                new Dictionary<string, Action<string, string>> {
                    { "reset", GatewayController.Reset },
                };


            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.TaskFeedback, (m) => MessageProcessor.MapFeedbackToAction(m, actions), AppGroupId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Common, (m) => MessageProcessor.MapMessageToAction(m, commonActions), AppGroupId)
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
