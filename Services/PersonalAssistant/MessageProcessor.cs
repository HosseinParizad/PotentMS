using System;
using System.Collections.Generic;
using System.Text.Json;
using PotentHelper;

namespace PersonalAssistant
{
    internal class MessageProcessor
    {
        public static void MessageReceived(string message)
        {
            var actions = new Dictionary<string, Action<string, string>> {
                { "taskFeedback", Engine.OnTaskFeedback },
            };

            try
            {
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
                        Console.WriteLine($"*****************pa****************************************** cannot run action. {message}");
                    }
                }
                else
                {
                    //Console.WriteLine($"**********************pa************************************* action is not specified. {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"*************************pa********************************** format is wrong. {message}");
            }
        }
    }
}
