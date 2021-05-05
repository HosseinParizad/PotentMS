using System;
using System.Collections.Generic;
using System.Text.Json;
using PotentHelper;

namespace iTodo
{
    internal class MessageProcessor
    {
        public static void MessageReceived(string message)
        {
            try
            {
                var msg = (Msg)JsonSerializer.Deserialize(message, typeof(Msg));
                if (Helper.TaskAction.TryGetValue(msg.Action, out var action))
                {
                    try
                    {
                        action(msg.GroupKey, msg.Content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"*********************************************************** cannot run action. {message}");
                    }
                }
                else
                {
                    Console.WriteLine($"*********************************************************** action is not specified. {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"*********************************************************** format is wrong. {message}");
            }
        }
    }
}
