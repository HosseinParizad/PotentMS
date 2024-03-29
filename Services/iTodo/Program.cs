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

    public class SetupActions : SetupActionsCore
    {
        public override List<MapBinding> Mapping => new()
        {
            new MapBinding(MapAction.Common.Reset, Engine.Reset),
            new MapBinding(MapAction.Task.NewTask, Engine.CreateNewTask),
            new MapBinding(MapAction.Task.UpdateDescription, Engine.UpdateDescription),
            new MapBinding(MapAction.Task.SetDeadline, Engine.SetDeadline),
            new MapBinding(MapAction.Task.SetTag, Engine.SetTag),
            new MapBinding(MapAction.Task.SetLocation, Engine.SetLocation),
            new MapBinding(MapAction.Task.CloseTask, Engine.CloseTask),
            new MapBinding(MapAction.Task.AssignTask, Engine.AssignTask),
            new MapBinding(MapAction.Task.DelTask, Engine.DeleteTask),
            new MapBinding(MapAction.Task.MoveTask, Engine.MoveTask),
            new MapBinding(MapAction.RepeatFeedback.RepeatNewItem, Engine.RepeatTask),
        };

        public override string AppGroupId => "iTodo";
    }
}