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
                    try
                    {
                        action(msg.GroupKey, msg.Content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"*********************************************************** cannot run action. {messege}");
                    }
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
