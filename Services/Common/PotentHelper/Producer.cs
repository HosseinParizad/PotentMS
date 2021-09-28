using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace PotentHelper
{
    public class ProducerHelper
    {
        public delegate void SendAMessageEventHandler(object sender, FullMessage e);

        public static event SendAMessageEventHandler OnSendAMessageEvent;

        //public static async Task SendAMessage(string topic, Msg msg)
        //{
        //    await SendAMessage(new FullMessage(topic, msg));
        //}

        public static async Task SendMessage(string topic, Msg msg)
        {
            var metadata = msg.Metadata;
            if (metadata.GroupKey == null || metadata.MemberKey == null)
            {
                throw new ArgumentException("Group and Member key should specify!");
            }
            await SendMessage(new FullMessage(topic, msg));
        }

        public static async Task SendMessage(string topic, Feedback feedback)
        {
            await SendMessage(new FullMessage(topic, feedback));
        }

        public static async Task SendMessage(FullMessage message)
        {

            if (OnSendAMessageEvent != null)
            {
                OnSendAMessageEvent.Invoke(null, new FullMessage(message.Topic, message.Message));
            }
            else
            {
                // try
                // {
                //     metadata.ReferenceKey = metadata.ReferenceKey;
                // }
                // catch (Exception)
                // {
                //     message.Message.Metadata = JsonConvert.DeserializeAnonymousType<dynamic>(message.Message.Metadata.ToString(), message.Message.Metadata);
                //     message.Message.Content = JsonConvert.DeserializeAnonymousType<dynamic>(message.Message.Content.ToString(), message.Message.Content);
                // }

                // Console.WriteLine("111111111111111111111111111");
                // Console.WriteLine(JsonConvert.SerializeObject(message));
                // Console.WriteLine(message.Message.Content);

                // message.Message.Metadata = JsonConvert.DeserializeAnonymousType<dynamic>(message.Message.Metadata.ToString(), message.Message.Metadata);
                // message.Message.Content = JsonConvert.DeserializeAnonymousType<dynamic>(message.Message.Content.ToString(), message.Message.Content);
                // Console.WriteLine(message.Message.Metadata);
                // Console.WriteLine(message.Message.Content);

                var metadata = message.Message.Metadata;
                if (metadata.ReferenceKey == null)
                {
                    metadata.ReferenceKey = Guid.NewGuid().ToString();
                }
                if (metadata.CreateDate == null)
                {
                    metadata.CreateDate = DateTimeOffset.Now;
                }
                if (metadata.Version == null)
                {
                    metadata.Version = "V0.0";
                }
                var msg = JsonConvert.SerializeObject(message.Message);
                var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

                // If serializers are not specified, default serializers from
                // `Confluent.Kafka.Serializers` will be automatically used where
                // available. Note: by default strings are encoded as UTF8.
                using (var p = new ProducerBuilder<Null, string>(config).Build())
                {
                    try
                    {
                        var dr = await p.ProduceAsync(KafkaEnviroment.preFix + message.Topic, new Message<Null, string> { Value = msg });
                        //Console.WriteLine($"{KafkaEnviroment.preFix + message.Topic} -> Delivered");
                    }
                    catch (ProduceException<Null, string> e)
                    {
                        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    }
                }
            }
        }
    }
}