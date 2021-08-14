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

        public static void Main(string[] args)
        {
            KafkaEnviroment.TempPrefix = args[0];
            var AppId = KafkaEnviroment.preFix + AppGroupId + (KafkaEnviroment.preFix == "" ? "" : Guid.NewGuid().ToString());

            var repeatActions =
                new Dictionary<string, Action<dynamic, dynamic>>
                {
                    { MapAction.Repeat.RegisterRepeat, Engine.RegisterRepeat },
                };

            var commonActions =
                new Dictionary<string, Action<dynamic, dynamic>> {
                    { "reset", Engine.Reset },
                };

            Parallel.Invoke(
                () => CreateHostBuilder(args).Build().Run(),
                ConsumerHelper.MapTopicToMethod(MessageTopic.Repeat, (m) => MessageProcessor.MapMessageToAction(AppId, m, repeatActions), AppId),
                ConsumerHelper.MapTopicToMethod(MessageTopic.Common, (m) => MessageProcessor.MapMessageToAction(AppId, m, commonActions), AppId)
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
