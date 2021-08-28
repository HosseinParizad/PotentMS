using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PotentHelper;

namespace RepeatManager
{
    public class Program
    {
        const string AppGroupId = "RepeatManager";
        public static DateTimeOffset StartingTimeApp;
        static DbText db = new();

        public static void Main(string[] args)
        {
            StartingTimeApp = DateTimeOffset.Now;
            KafkaEnviroment.TempPrefix = args[0];
            var AppId = KafkaEnviroment.preFix + AppGroupId;

            var repeatActions =
                new Dictionary<string, Action<dynamic, dynamic>>
                {
                    { MapAction.Repeat.RegisterRepeat, Engine.RegisterRepeat },
                };

            var commonActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", Engine.Reset },
                };

            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;

            void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
            {
                MessageProcessor.MapMessageToAction(AppId, e.Text, commonActions, true);
                MessageProcessor.MapMessageToAction(AppId, e.Text, repeatActions, true);
            }

            db.ReplayAll();

            Parallel.Invoke(
                () => CreateHostBuilder(args).Build().Run(),
                ConsumerHelper.MapTopicToMethod(new[] { MessageTopic.Repeat, MessageTopic.Common }, (m) => MessageProcessor.MapMessageToAction(AppId, m, (m) => db.Add(m)), AppId)
            );

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
