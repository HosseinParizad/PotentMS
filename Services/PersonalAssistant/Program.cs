using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Collections.Generic;

namespace PersonalAssistant
{
    public partial class Program
    {
        public static DateTimeOffset StartingTimeApp;

        public static void Main(string[] args)
        {
            StartingTimeApp = DateTimeOffset.Now;
            KafkaEnviroment.TempPrefix = args[0];

            var setupActions = new SetupActions();
            setupActions.Ini();

            CreateHostBuilder(args).Build().Run();
        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public class SetupActions
    {
        const string AppGroupId = "PersonalAssistant";
        public DbText db = new();
        string AppId = KafkaEnviroment.preFix + AppGroupId;

        Dictionary<string, Action<dynamic, dynamic>> taskFeedbackActions =
            new Dictionary<string, Action<dynamic, dynamic>>
            {
                   { FeedbackActions.NewGoalAdded, Engine.ApplyNewTaskAdded },
            };

        Dictionary<string, Action<dynamic, dynamic>> commonActions =
            new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", Engine.Reset },
            };

        Dictionary<string, Action<dynamic, dynamic>> locationActions =
            new Dictionary<string, Action<dynamic, dynamic>> {
                    { "setCurrentLocation", Engine.SetCurrentLocation },
            };

        Dictionary<string, Action<dynamic, dynamic>> groupFeedbackActions =
            new Dictionary<string, Action<dynamic, dynamic>>
            {
                    { FeedbackActions.NewGoalAdded, Engine.ApplyNewGroupAdded },
            };


        public void Ini()
        {


            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            if (KafkaEnviroment.TempPrefix == "Test")
            {
                db.ReplayAll();
            }

            ConsumerHelper.MapTopicToMethod(
                new[] { MessageTopic.GroupFeedback, MessageTopic.Common }
                , (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId);
        }

        public void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
        {
            MessageProcessor.MapMessageToAction(AppId, e.Text, commonActions);
            MessageProcessor.MapMessageToAction(AppId, e.Text, locationActions);
            MessageProcessor.MapMessageToAction(AppId, e.Text, taskFeedbackActions);
            MessageProcessor.MapMessageToAction(AppId, e.Text, groupFeedbackActions);
        }

    }
}
