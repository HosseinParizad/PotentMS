using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PotentHelper;

namespace iLocation
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public DateTimeOffset Now => DateTimeOffset.Now;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", Now);
                await Task.Delay(10000, stoppingToken);

                //Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                //Console.WriteLine(JsonConvert.SerializeObject(Engine.Repeat.FirstOrDefault()));
                //Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

                if (Engine.MemberRegisterd && Engine.ReadLastLocation() != Engine.LastLocation)
                {
                    var lastRead = Engine.ReadLastLocation();
                    if (lastRead != Engine.LastLocation)
                    {
                        Engine.LastLocation = lastRead;
                        Engine.MemeberMoveToNewLocation(Engine.LastLocation);
                    }
                }
            }
        }
    }
}
