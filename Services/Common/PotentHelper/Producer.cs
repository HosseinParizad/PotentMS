using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace PotentHelper
{
    public class ProducerHelper
    {
        public static async Task SendAMessage(string topic, Msg obj)
        {
            var dyn = Helper.DeserializeObject<dynamic>(System.Text.Json.JsonSerializer.Serialize(obj)); //Todo:remove
            await SendMessageCore(topic, dyn);
        }

        public static async Task SendAMessage(string topic, Feedback obj)
        {
            var dyn = Helper.DeserializeObject<dynamic>(System.Text.Json.JsonSerializer.Serialize(obj)); //Todo:remove
            await SendMessageCore(topic, dyn);
        }

        static async Task SendMessageCore(string topic, dynamic obj)
        {
            var metadata = obj.Metadata;
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
            var msg = JsonConvert.SerializeObject(obj);
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

            // If serializers are not specified, default serializers from
            // `Confluent.Kafka.Serializers` will be automatically used where
            // available. Note: by default strings are encoded as UTF8.
            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync(KafkaEnviroment.preFix + topic, new Message<Null, string> { Value = msg });
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