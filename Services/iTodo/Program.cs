using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PotentHelper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace iTodo
{
    public partial class Program
    {
        const string AppGroupId = "iTodo";

        public static void Main(string[] args)
        {
            KafkaEnviroment.TempPrefix = args[0];
            var AppId = KafkaEnviroment.preFix + AppGroupId;

            #region  actions

            var taskActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
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
                    { "startTask", Engine.StartTask },
                    { "pauseTask", Engine.PauseTask },
                };

            #endregion

            var locationActions = new Dictionary<string, Action<dynamic, dynamic>> { { MapAction.Location.SetCurrentLocation, Engine.SetCurrentLocation }, };

            var commonActions = new Dictionary<string, Action<dynamic, dynamic>> { { "reset", Engine.Reset }, };

            var repeatActions = new Dictionary<string, Action<Feedback>> { { MapAction.Task.RepeatTask, Engine.RepeatTask } };

            var source = new CancellationTokenSource();
            var token = source.Token;
            Parallel.Invoke(
                    () => CreateHostBuilder(args).Build().Run(),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Task, (m) => MessageProcessor.MapMessageToAction(AppId, m, taskActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Location, (m) => MessageProcessor.MapMessageToAction(AppId, m, locationActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.Common, (m) => MessageProcessor.MapMessageToAction(AppId, m, commonActions), AppId),
                    ConsumerHelper.MapTopicToMethod(MessageTopic.RepeatFeedback, (m) => MessageProcessor.MapFeedbackToAction(AppId, m, repeatActions), AppId)
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
