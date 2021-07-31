using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PotentHelper;

namespace RepeatManager
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

                foreach (var item in Engine.Repeat.Where(r => r.NextGeneratedTime < Now).ToArray())
                {
                    if (item.ReferenceName == "Task")
                    {
                        var now = Now;
                        var dataToSend = new { Id = item.ReferenceId, LastGeneratedTime = now };
                        item.LastGeneratedTime = now;
                        SendAMessage(type: FeedbackType.Apply, action: MapAction.Task.RepeatTask, content: JsonSerializer.Serialize(dataToSend));
                    }
                }
            }
        }

        static void SendAMessage(FeedbackType type, string action, string content)
            => ProducerHelper.SendAMessage(MessageTopic.RepeatFeedback, JsonSerializer.Serialize(new Feedback(type: type, name: action, action: action, key: "", content: content))).GetAwaiter().GetResult();

    }
}