using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Gateway.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PotentHelper;

namespace Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Parallel.Invoke(
                () => CreateHostBuilder(args).Build().Run(),
                () =>
                {
                    var source = new CancellationTokenSource();
                    var token = source.Token;

                    var topics = new List<string>() { "taskFeedback" };
                    var iTodoConsumer = new ConsumerHelper("localhost:9092", topics, token, GatewayController.MessageReceived);
                }
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
