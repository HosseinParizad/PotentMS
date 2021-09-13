using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Collections.Generic;

namespace iTodo
{
    public partial class Program
    {
        public static DateTimeOffset StartingTimeApp;

        public static void Main(string[] args)
        {
            StartingTimeApp = DateTimeOffset.Now;
            KafkaEnviroment.TempPrefix = args[0];

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
        const string AppGroupId = "iTodo";
        public DbText db = new();
        public void Ini()
        {
            var AppId = KafkaEnviroment.preFix + AppGroupId;

            #region  actions

            var taskActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { MapAction.Task.NewTask, Engine.CreateNewTask },
                    { MapAction.Task.UpdateDescription, Engine.UpdateDescription },
                    { MapAction.Task.SetDeadline, Engine.SetDeadline },
                    { MapAction.Task.SetTag, Engine.SetTag },
                    { MapAction.Task.SetLocation, Engine.SetLocation },
                    { MapAction.Task.CloseTask, Engine.CloseTask },
                    { MapAction.Task.AssignTask, Engine.AssignTask },
                    { MapAction.Task.DelTask, Engine.DeleteTask },
                    { MapAction.Task.MoveTask, Engine.MoveTask },
                };

            #endregion

            var locationActions = new Dictionary<string, Action<dynamic, dynamic>> { { MapAction.Location.SetCurrentLocation, Engine.SetCurrentLocation }, };

            var commonActions = new Dictionary<string, Action<dynamic, dynamic>> { { "reset", Engine.Reset }, };

            var repeatActions = new Dictionary<string, Action<Feedback>> { { MapAction.Task.RepeatTask, Engine.RepeatTask } };

            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
            {
                MessageProcessor.MapMessageToAction(AppId, e.Text, taskActions, true);
                MessageProcessor.MapMessageToAction(AppId, e.Text, locationActions, true);
                MessageProcessor.MapMessageToAction(AppId, e.Text, commonActions, true);
                MessageProcessor.MapFeedbackToAction(AppId, e.Text, repeatActions, true);
            }

            if (KafkaEnviroment.TempPrefix == "Test")
            {
                db.ReplayAll();
            }

            ConsumerHelper.MapTopicToMethod(new[]
                                {
                        MessageTopic.Task,
                        MessageTopic.Location,
                        MessageTopic.Common,
                        MessageTopic.RepeatFeedback
                    }, (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId);
        }
    }
}
