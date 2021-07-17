using System;
using System.Collections.Generic;
using System.Text.Json;

namespace PotentHelper
{
    public class MessageProcessor
    {
        public static void MapMessageToAction(string appId, string message, Dictionary<string, Action<string, string>> actions)
        {
            try
            {
                var msg = (IMessageContract)JsonSerializer.Deserialize(message, typeof(Msg));
                //Console.WriteLine($"-------------------{msg.Action}------{string.Join("|", actions.Keys)}-----------");
                if (actions.TryGetValue(msg.Action, out var action))
                {
                    try
                    {
                        action(msg.Key, msg.Content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"<><><><> {appId} <><><><> cannot run action. {message}");
                    }
                }
                else
                {
                    Console.WriteLine($"<><><><> {appId} <><><><> action is not specified. {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($" <><><><> {appId} <><><><> format is wrong. {message}");
            }
        }

        public static void MapFeedbackToAction(string appId, string message, Dictionary<string, Action<Feedback>> actions)
        {
            try
            {
                var msg = (Feedback)JsonSerializer.Deserialize(message, typeof(Feedback));
                //Console.WriteLine($"-------------------{msg.Name}------{string.Join("|", actions.Keys)}-----------");
                if (actions.TryGetValue(msg.Name, out var action))
                {
                    try
                    {
                        action(msg);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"*{appId}%%%%%% Feedback %%%%% cannot run action. {message}");
                    }
                }
                //else
                //{
                //    Console.WriteLine($%%%%% Feedback %%%%% action is not specified. {message}");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"*{appId}%%%%% Feedback %%%%% format is wrong. {message}");
            }
        }
    }
}
