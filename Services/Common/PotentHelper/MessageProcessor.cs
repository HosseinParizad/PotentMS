using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PotentHelper
{
    public class MessageProcessor
    {
        public static void MapMessageToAction(string appId, string message, Dictionary<string, Action<dynamic, dynamic>> actions)
        {
            try
            {
                if (message.Length < 10)
                {
                    return;
                }

                var msg = Helper.DeserializeObject<Msg>(message);
                if (actions.TryGetValue(msg.Action, out var action))
                {
                    try
                    {
                        action(msg.Metadata, msg.Content);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine($"<><><><> {appId} <><><><> cannot run action. {message}");
                    }
                }
                //else if (!ignoreMissingAction)
                //{
                //    Console.WriteLine($"<><><><> {appId} <><><><> action is not specified. {message}");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($" <><><><> {appId} <><><><> format is wrong. {message}");
            }
        }

        //public static void MapFeedbackToAction(string appId, string message, Dictionary<string, Action<Feedback>> actions, bool ignoreMissingAction = true)
        //{
        //    try
        //    {
        //        if (message.Length < 10)
        //        {
        //            return;
        //        }

        //        var msg = Helper.DeserializeObject<Feedback>(message);
        //        if (actions.TryGetValue(msg.Action, out var action))
        //        {
        //            try
        //            {
        //                action(msg);
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine(ex.Message);
        //                Console.WriteLine($"*{appId}%%%%%% Feedback %%%%% cannot run action. {message}");
        //            }
        //        }
        //        else if (!ignoreMissingAction)
        //        {
        //            Console.WriteLine($"*{appId}%%%%%% Feedback is not specified. {message}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.WriteLine($"*{appId}%%%%% Feedback %%%%% format is wrong. {message}");
        //    }
        //}

        public static void MapMessageToAction(string appId, string message, Action<string> action)
        {
            try
            {
                if (message.Length < 10)
                {
                    return;
                }
                action(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"|||||| {appId} ||||| cannot run action. {message}");
            }
        }
    }
}