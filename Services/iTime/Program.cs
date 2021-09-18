using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iTime
{
    public class Program
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
        const string AppTimeId = "iTime";
        public DbText db = new();
        string AppId = KafkaEnviroment.preFix + AppTimeId;

        Dictionary<string, Action<dynamic, dynamic>> actions =
            new Dictionary<string, Action<dynamic, dynamic>>
            {
                    { MapAction.Time.Start, Engine.StartTask },
                    { MapAction.Time.Pause, Engine.PauseTask },
                    { MapAction.Time.Done, Engine.DoneTask },
            };

        Dictionary<string, Action<dynamic, dynamic>> commonActions =
            new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", Engine.Reset },
            };

        public void Ini()
        {
            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            if (KafkaEnviroment.TempPrefix == "Test")
            {
                db.ReplayAll();
            }

            ConsumerHelper.MapTopicToMethod(new[] { MessageTopic.Time, MessageTopic.Common }, (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId);
        }

        public void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
        {
            MessageProcessor.MapMessageToAction(AppId, e.Text, actions);
            MessageProcessor.MapMessageToAction(AppId, e.Text, commonActions);
        }

    }
}
