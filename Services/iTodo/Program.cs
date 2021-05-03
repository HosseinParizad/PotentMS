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
            Parallel.Invoke(
                () => CreateHostBuilder(args).Build().Run(),
                () =>
                    {
                        var source = new CancellationTokenSource();
                        var token = source.Token;

                        //var topics = new List<string>() { "task", "location" };
                        _ = new ConsumerHelper("localhost:9092", new List<string>() { "task", "location" }, token, MessageProcessor.MessageReceived);
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
