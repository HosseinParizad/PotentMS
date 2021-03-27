using System;
using KafkaHelper;
using System.Threading;
using System.Collections.Generic;

namespace iTodo
{
    class Program
    {
        static void Main(string[] args)
        {
            var source = new CancellationTokenSource();
            var token = source.Token;

            var topics = new List<string>() { "task" };
            var iTodoConsumer = new Consumer("localhost:9092", topics, token, MessageReceived);
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }

        static void MessageReceived(string message)
        {
            Console.WriteLine($"i can hear you {message}");
        }
    }
}
