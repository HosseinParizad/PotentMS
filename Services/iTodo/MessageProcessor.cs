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
                var actions = new Dictionary<string, Action<string, string>> {
                    { "newTask", Engine.CreateNewTask },
                    { "updateDescription", Engine.UpdateDescription },
                    { "setDeadline", Engine.SetDeadline },
                    { "setTag", Engine.SetTag },
                    { "setCurrentLocation", Engine.SetCurrentLocation },
                    { "newGroup", Engine.NewGroup },
                    { "newMember", Engine.NewMember },
                    { "setLocation", Engine.SetLocation },
                    { "closeTask", Engine.CloseTask },
                 };

                var msg = (Msg)JsonSerializer.Deserialize(message, typeof(Msg));

                if (actions.TryGetValue(msg.Action, out var action))
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
