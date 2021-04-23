using System;
using System.Collections.Generic;
using System.Text.Json;
using PotentHelper;

namespace iTodo
{
    internal class MessageProcessor
    {
        public static void MessageReceived(string messege)
        {
            try
            {
                var msg = (Msg)JsonSerializer.Deserialize(messege, typeof(Msg));
                if (Helper.TaskAction.TryGetValue(msg.Action, out var action))
                {
                    action(msg.GroupKey, msg.Content);
                }
                else
                {
                    Console.WriteLine($"*********************************************************** action is not specified. {messege}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"*********************************************************** format is wrong. {messege}");
            }
        }
    }
}
