using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iLocation
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
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });

    }

    public class SetupActions
    {
        const string AppGroupId = "iLocation";
        public DbText db = new();
        public void Ini()
        {
            var AppId = KafkaEnviroment.preFix + AppGroupId;

            var actions =
                new Dictionary<string, Action<dynamic, dynamic>>
                {
                    { MapAction.Assistant.RegisterMember, Engine.RegisterMember },
                    { MapAction.Assistant.TestOnlyLocationChanged, Engine.TestOnlyLocationChanged },
                };

            var commonActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", Engine.Reset },
                };

            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
            {
                MessageProcessor.MapMessageToAction(AppId, e.Text, actions, true);
                MessageProcessor.MapMessageToAction(AppId, e.Text, commonActions, true);
            }

            if (KafkaEnviroment.TempPrefix == "Test")
            {
                db.ReplayAll();
            }

            ConsumerHelper.MapTopicToMethod(new[] { MessageTopic.Time, MessageTopic.Common }, (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId);
        }
    }
}
