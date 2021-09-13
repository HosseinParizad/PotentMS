using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iGroup
{
    public class Program
    {
        const string AppGroupId = "iGroup";
        public static DateTimeOffset StartingTimeApp;
        static DbText db = new();

        public static void Main(string[] args)
        {
            StartingTimeApp = DateTimeOffset.Now;
            KafkaEnviroment.TempPrefix = args[0];

            var setupActions = new SetupActions();
            setupActions.Ini();

            db.ReplayAll();

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
        const string AppGroupId = "iMemory";
        //public static DateTimeOffset StartingTimeApp;
        public DbText db = new();

        public void Ini()
        {
            var AppId = KafkaEnviroment.preFix + AppGroupId;

            var actions =
                new Dictionary<string, Action<dynamic, dynamic>>
                {
                    { MapAction.Group.NewGroup, Engine.CreateNewGroup },
                    { MapAction.Group.UpdateGroup, Engine.UpdateGroup },
                    { MapAction.Group.NewMember, Engine.AddMember },
                    { MapAction.Group.DeleteMember, Engine.DeleteMember },
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

            ConsumerHelper.MapTopicToMethod(new[] { MessageTopic.Group, MessageTopic.Common }, (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId);
        }
    }
}
