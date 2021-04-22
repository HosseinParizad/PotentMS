using System;
using System.Threading;
using Confluent.Kafka;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace PotentHelper
{
    public class ConsumerHelper
    {
        public ConsumerHelper(string brokerList, List<string> topics, CancellationToken cancellationToken, Action<string> onMessageReceived)
        {
            if (string.IsNullOrEmpty(brokerList))
            {
                throw new ArgumentException($"'{nameof(brokerList)}' cannot be null or empty.", nameof(brokerList));
            }

            if (topics is null)
            {
                throw new ArgumentNullException(nameof(topics));
            }

            if (onMessageReceived is null)
            {
                throw new ArgumentNullException(nameof(onMessageReceived));
            }

            BrokerList = brokerList;
            Topics = topics;
            CancellationToken = cancellationToken;
            OnMessageReceived = onMessageReceived;
            Listen();
        }

        void Listen()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = BrokerList,
                GroupId = "csharp-consumer",
                EnableAutoCommit = false,
                StatisticsIntervalMs = 5000,
                SessionTimeoutMs = 6000,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true,
                // A good introduction to the CooperativeSticky assignor and incremental rebalancing:
                // https://www.confluent.io/blog/cooperative-rebalancing-in-kafka-streams-consumer-ksqldb/
                PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky
            };

            const int commitPeriod = 5;

            // Note: If a key or value deserializer is not set (as is the case below), the 
            // deserializer corresponding to the appropriate type from Confluent.Kafka.Deserializers
            // will be used automatically (where available). The default deserializer for string
            // is UTF8. The default deserializer for Ignore returns null for all input data
            // (including non-null data).
            using (var consumer = new ConsumerBuilder<Ignore, string>(config)
                // Note: All handlers are called on the main .Consume thread.
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
                //.SetStatisticsHandler((_, json) => Console.WriteLine($"Statistics: {json}"))
                .SetPartitionsAssignedHandler((c, partitions) =>
                {
                    // Since a cooperative assignor (CooperativeSticky) has been configured, the
                    // partition assignment is incremental (adds partitions to any existing assignment).
                    Console.WriteLine($"Incremental partition assignment: [{string.Join(", ", partitions)}]");

                    // Possibly manually specify start offsets by returning a list of topic/partition/offsets
                    // to assign to, e.g.:
                    // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                })
                .SetPartitionsRevokedHandler((c, partitions) =>
                {
                    // Since a cooperative assignor (CooperativeSticky) has been configured, the revoked
                    // assignment is incremental (may remove only some partitions of the current assignment).
                    Console.WriteLine($"Incremental partition revokation: [{string.Join(", ", partitions)}]");
                })
                .SetPartitionsLostHandler((c, partitions) =>
                {
                    // The lost partitions handler is called when the consumer detects that it has lost ownership
                    // of its assignment (fallen out of the group).
                    Console.WriteLine($"Partitions were lost: [{string.Join(", ", partitions)}]");
                })
                .Build())
            {
                consumer.Subscribe(Topics);

                try
                {
                    while (true)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume(CancellationToken);

                            if (consumeResult.IsPartitionEOF)
                            {
                                Console.WriteLine(
                                    $"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                continue;
                            }

                            OnMessageReceived(consumeResult.Message.Value);
                            Console.WriteLine($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Message.Value}");

                            if (consumeResult.Offset % commitPeriod == 0)
                            {
                                // The Commit method sends a "commit offsets" request to the Kafka
                                // cluster and synchronously waits for the response. This is very
                                // slow compared to the rate at which the consumer is capable of
                                // consuming messages. A high performance application will typically
                                // commit offsets relatively infrequently and be designed handle
                                // duplicate messages in the event of failure.
                                try
                                {
                                    consumer.Commit(consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    Console.WriteLine($"Commit error: {e.Error.Reason}");
                                }
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Consume error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Closing consumer.");
                    consumer.Close();
                }
            }
        }

        #region prop

        string BrokerList { get; }
        List<string> Topics { get; }
        CancellationToken CancellationToken { get; }
        Action<string> OnMessageReceived { get; }

        #endregion
    }
}