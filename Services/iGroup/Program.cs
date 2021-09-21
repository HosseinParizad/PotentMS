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
        const string AppGroupId = "iGroup";
        public string AppId = KafkaEnviroment.preFix + AppGroupId;
        public DbText db = new();

        public List<MapBinding> mapping = new List<MapBinding>()
        {
            new MapBinding(MapAction.Common.Reset, Engine.Reset),
            new MapBinding(MapAction.Group.NewGroup, Engine.CreateNewGroup),
            new MapBinding(MapAction.Group.UpdateGroup, Engine.UpdateGroup),
            new MapBinding(MapAction.Group.NewMember, Engine.AddMember),
            new MapBinding(MapAction.Group.DeleteMember, Engine.DeleteMember),
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