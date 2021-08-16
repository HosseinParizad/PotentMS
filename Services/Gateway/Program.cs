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
        public static DateTimeOffset StartingTimeApp;

        public static void Main(string[] args)
        {
            StartingTimeApp = DateTimeOffset.Now;
            KafkaEnviroment.TempPrefix = args[0];
            var AppId = KafkaEnviroment.preFix + AppGroupId + (KafkaEnviroment.preFix == "" ? "" : Guid.NewGuid().ToString());

            var commonActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", GatewayController.Reset },
                };


            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.TaskFeedback, (m) => MessageProcessor.MapFeedbackToAction(AppId, m, actions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.PersonalAssistantFeedback, (m) => MessageProcessor.MapFeedbackToAction(AppId, m, new Dictionary<string, Action<Feedback>> { { FeedbackGroupNames.PersonalAssistant, GatewayController.PAMessageReceived } }), AppId),
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
