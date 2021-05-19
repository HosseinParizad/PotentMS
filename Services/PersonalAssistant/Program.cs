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

namespace PersonalAssistant
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parallel.Invoke(
                () => CreateHostBuilder(args).Build().Run(),
                //ConsumerHelper.MapTopicToMethod("task", (m) => MessageProcessor.MapMessageToAction(m, actions)),
                //ConsumerHelper.MapTopicToMethod("location", (m) => MessageProcessor.MapMessageToAction(m, actions)),
                ConsumerHelper.MapTopicToMethod("taskFeedback", (m) => MessageProcessor.MapMessageToAction(m, actions))
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
                { "taskFeedback", Engine.OnTaskFeedback },
            };

    }
}
