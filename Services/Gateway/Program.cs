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
        static DbText db = new();

        public static void Main(string[] args)
        {
            StartingTimeApp = DateTimeOffset.Now;
            KafkaEnviroment.TempPrefix = args[0];
            var AppId = KafkaEnviroment.preFix + AppGroupId;

            var commonActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", GatewayController.Reset },
                };


            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
            {
                MessageProcessor.MapMessageToAction(AppId, e.Text, commonActions, true);
                MessageProcessor.MapFeedbackToAction(AppId, e.Text, actions, true);
                //MessageProcessor.MapFeedbackToAction(AppId, e.Text, new Dictionary<string, Action<Feedback>> { { FeedbackGroupNames.PersonalAssistant, GatewayController.PAMessageReceived } }, true);
            }

            db.ReplayAll();

            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(new[]
                    {
                        MessageTopic.TaskFeedback, MessageTopic.PersonalAssistantFeedback, MessageTopic.MemoryFeedback, MessageTopic.Common
                    }
                    , (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId)
                ); ;
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
                { FeedbackGroupNames.Memory, GatewayController.MessageReceived },
    };

    }
}
