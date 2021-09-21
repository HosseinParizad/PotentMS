using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public string AppId = KafkaEnviroment.preFix + AppGroupId;

        public List<MapBinding> mapping = new List<MapBinding>()
        {
            new MapBinding(MapAction.Common.Reset, Engine.Reset),
        };


        public async void Ini()
        {

            db.Initial(AppId + "DB.txt");
            db.OnDbNewDataEvent += Db_DbNewDataEvent;
            await MapAsync();
        }

        public void Db_DbNewDataEvent(object sender, DbNewDataEventArgs e)
        {
            MessageProcessor.MapMessageToAction(AppId, e.Text, mapping);
        }

        async Task MapAsync() => await Task.Run(() => ConsumerHelper.MapTopicToMethod(mapping, db, AppId).ToList());
    }
}