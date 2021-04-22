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
                        try
                        {
                            var source = new CancellationTokenSource();
                            var token = source.Token;

                            var topics = new List<string>() { "task" };
                            var iTodoConsumer = new ConsumerHelper("localhost:9092", topics, token, MessageProcessor.MessageReceived);

                        }
                        catch (System.Exception)
                        {
                        }

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
