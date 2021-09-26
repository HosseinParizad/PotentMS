using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iGoal
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

            var source = new CancellationTokenSource();
            var token = source.Token;

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
            new MapBinding(MapAction.Goal.NewGoal, Engine.CreateNewGoal),
            new MapBinding(MapAction.Goal.DelGoal, Engine.DeleteGoal),
            new MapBinding(MapAction.Goal.UpdateGoal, Engine.UpdateGoal),
        };

        public override string AppGroupId => "iGoal";
    }
}
