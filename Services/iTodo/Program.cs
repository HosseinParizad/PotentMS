using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace iTodo
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

            //Console.WriteLine("ooooooooooooooooooooooooooooooooooooooooooo");
            var source = new CancellationTokenSource();
            var token = source.Token;

            //_ = ConsumerHelper.MapTopicToMethod(setupActions.mapping, setupActions.db, setupActions.AppId);
            //_ = new ConsumerHelper("localhost:9092", new List<string>() { "TestTask" }, token,(m) => setupActions.db.Add(m), "TTooooT");
            //_ = ConsumerHelper.MapTopicToMethod(setupActions.mapping, setupActions.db, setupActions.AppId).ToList();

            CreateHostBuilder(args).Build().Run();
            //Parallel.Invoke(
            //        () => CreateHostBuilder(args).Build().Run(),
            //        ConsumerHelper.MapTopicToMethod(setupActions.mapping, setupActions.db, setupActions.AppId)
            //    );

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
        public string AppId = KafkaEnviroment.preFix + AppGroupId;

        #region  actions

        public List<MapBinding> mapping = new List<MapBinding>()
        {
            new MapBinding(MapAction.Common.Reset, Engine.Reset),
            new MapBinding(MapAction.Task.NewTask, Engine.CreateNewTask ),
            new MapBinding(MapAction.Task.UpdateDescription, Engine.UpdateDescription ),
            new MapBinding(MapAction.Task.SetDeadline, Engine.SetDeadline ),
            new MapBinding(MapAction.Task.SetTag, Engine.SetTag ),
            new MapBinding(MapAction.Task.SetLocation, Engine.SetLocation ),
            new MapBinding(MapAction.Task.CloseTask, Engine.CloseTask ),
            new MapBinding(MapAction.Task.AssignTask, Engine.AssignTask ),
            new MapBinding(MapAction.Task.DelTask, Engine.DeleteTask ),
            new MapBinding(MapAction.Task.MoveTask, Engine.MoveTask ),
            new MapBinding(MapAction.RepeatFeedback.RepeatNewItem , Engine.RepeatTask ),
        };

        #endregion


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