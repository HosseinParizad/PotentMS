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
        public static DateTimeOffset StartingTimeApp;
        static DbText db = new();

        public static void Main(string[] args)
        {
            StartingTimeApp = DateTimeOffset.Now;
            KafkaEnviroment.TempPrefix = args[0];
            var AppId = KafkaEnviroment.preFix + AppGroupId;

            var taskFeedbackActions =
                new Dictionary<string, Action<Feedback>>
                {
                   { FeedbackGroupNames.Task, Engine.OnTaskFeedback },
                };

            var memoryFeedbackActions =
                new Dictionary<string, Action<Feedback>>
                {
                    { FeedbackGroupNames.Memory, Engine.OnMemoryFeedback },
                };

            var commonActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", Engine.Reset },
                };

            var locationActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "setCurrentLocation", Engine.SetCurrentLocation },
                };

            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
            {
                MessageProcessor.MapMessageToAction(AppId, e.Text, commonActions, true);
                MessageProcessor.MapMessageToAction(AppId, e.Text, locationActions, true);
                MessageProcessor.MapFeedbackToAction(AppId, e.Text, taskFeedbackActions, true);
                MessageProcessor.MapFeedbackToAction(AppId, e.Text, memoryFeedbackActions, true);
            }

            db.ReplayAll();

            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(
                        new[] { MessageTopic.TaskFeedback, MessageTopic.MemoryFeedback, MessageTopic.Memory, MessageTopic.Common, MessageTopic.Location }
                        , (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId)
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
