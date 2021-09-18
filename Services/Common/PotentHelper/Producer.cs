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

        public static async Task SendAMessage(string topic, Msg msg)
        {
            await SendAMessage(new FullMessage(topic, msg));
        }

        public static async Task SendAMessage(FullMessage message)
        {
            if (OnSendAMessageEvent != null)
            {
                OnSendAMessageEvent.Invoke(null, new FullMessage(message.Topic, message.Message));
            }
            else
            {
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
                        //Console.WriteLine($"{topic} -> Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
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