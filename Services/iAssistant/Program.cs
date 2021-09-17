using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Collections.Generic;

namespace iAssistant
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
        const string AppGroupId = "iAssistant";
        public DbText db = new();
        string AppId = KafkaEnviroment.preFix + AppGroupId;

        Dictionary<string, Action<dynamic, dynamic>> commonActions = new Dictionary<string, Action<dynamic, dynamic>> { { "reset", Engine.Reset }, };
        Dictionary<string, Action<Feedback>> taskActions = new Dictionary<string, Action<Feedback>> { { FeedbackActions.NewTaskAdded, Engine.TaskFeedback } };

        public void Ini()
        {

            #region  actions

            //var taskActions =
            //    new Dictionary<string, Action<dynamic, dynamic>> {
            //        { MapAction.Task.NewTask, Engine.CreateNewTask },
            //    };

            #endregion


            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            if (KafkaEnviroment.TempPrefix == "Test")
            {
                db.ReplayAll();
            }

            ConsumerHelper.MapTopicToMethod(new[]
                                {
                        MessageTopic.TaskFeedback,
                        MessageTopic.LocationFeedback,
                    }, (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId);
        }

        public void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
        {
            MessageProcessor.MapMessageToAction(AppId, e.Text, commonActions, true);
            MessageProcessor.MapFeedbackToAction(AppId, e.Text, taskActions, true);
        }

    }
}
