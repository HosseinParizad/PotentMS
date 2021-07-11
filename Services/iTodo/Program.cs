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
        const string AppGroupId = "iTodo";
        static string AppId = AppGroupId + Guid.NewGuid().ToString();


        public static void Main(string[] args)
        {
            var taskActions =
                new Dictionary<string, Action<string, string>> {
                    { "newTask", Engine.CreateNewTask },
                    { "newGoal", Engine.CreateNewGoal },
                    { "updateDescription", Engine.UpdateDescription },
                    { "setDeadline", Engine.SetDeadline },
                    { "setTag", Engine.SetTag },
                    { "newGroup", Engine.NewGroup },
                    { "newMember", Engine.NewMember },
                    { "setLocation", Engine.SetLocation },
                    { "closeTask", Engine.CloseTask },
                    { "assignTask", Engine.AssignTask },
                    { "delTask", Engine.DeleteTask },
                };

            var locationActions =
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

            var commonActions =
                new Dictionary<string, Action<string, string>> {
                    { "reset", Engine.Reset },
                };

            var source = new CancellationTokenSource();
            var token = source.Token;
            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Task, (m) => MessageProcessor.MapMessageToAction(m, taskActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Location, (m) => MessageProcessor.MapMessageToAction(m, locationActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Common, (m) => MessageProcessor.MapMessageToAction(m, commonActions), AppId)
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
