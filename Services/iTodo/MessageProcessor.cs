using System;
using System.Collections.Generic;
using System.Text.Json;
using KafkaHelper;

namespace iTodo
{
    internal class MessageProcessor
    {
        public static void MessageReceived(string messege)
        {
            try
            {
                var msg = (Msg)JsonSerializer.Deserialize(messege, typeof(Msg));
                if (Helper.TaskAction.TryGetValue(msg.Action, out var a))
                {
                    a(msg.BelongTo, msg.Content);
                }
                else
                {
                    throw new Exception($"action is not specified. {messege}");
                }
                Console.WriteLine($":| , i can hear you {messege}");
            }
            catch (System.Exception)
            {
                throw new Exception($"format is wrong. {messege}");
            }
        }
    }
}
