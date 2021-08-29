using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iMemory
{
    public class Program
    {
        const string AppGroupId = "iMemory";
        public static DateTimeOffset StartingTimeApp;
        static DbText db = new();

        public static void Main(string[] args)
        {
            StartingTimeApp = DateTimeOffset.Now;
            KafkaEnviroment.TempPrefix = args[0];
            var AppId = KafkaEnviroment.preFix + AppGroupId;

            var actions =
                new Dictionary<string, Action<dynamic, dynamic>>
                {
                    { MapAction.Memory.NewMemory, Engine.CreateNewMemory },
                    { MapAction.Memory.NewMemoryCategory, Engine.CreateMemoryCategory },
                    { MapAction.Memory.DelMemory, Engine.DeleteMemory },
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

            db.ReplayAll();

            Parallel.Invoke(
                () => CreateHostBuilder(args).Build().Run(),
                ConsumerHelper.MapTopicToMethod(new[] { MessageTopic.Memory, MessageTopic.Common }, (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId)
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
