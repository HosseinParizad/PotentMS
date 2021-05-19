using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PotentHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace iTodo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;
            Parallel.Invoke(
                () => CreateHostBuilder(args).Build().Run(),
                ConsumerHelper.MapTopicToMethod("task", (m) => MessageProcessor.MapMessageToAction(m, actions)),
                ConsumerHelper.MapTopicToMethod("location", (m) => MessageProcessor.MapMessageToAction(m, actions))
            );
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static Dictionary<string, Action<string, string>> actions =
                new Dictionary<string, Action<string, string>> {
                    { "newTask", Engine.CreateNewTask },
                    { "updateDescription", Engine.UpdateDescription },
                    { "setDeadline", Engine.SetDeadline },
                    { "setTag", Engine.SetTag },
                    { "setCurrentLocation", Engine.SetCurrentLocation },
                    { "newGroup", Engine.NewGroup },
                    { "newMember", Engine.NewMember },
                    { "setLocation", Engine.SetLocation },
                    { "closeTask", Engine.CloseTask },
                };


    }
}
