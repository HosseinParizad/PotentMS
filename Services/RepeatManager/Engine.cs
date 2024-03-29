﻿using System;
using System.Collections.Generic;
using PotentHelper;

namespace RepeatManager
{
    internal class Engine
    {
        public static void RegisterRepeat(dynamic metadata, dynamic content)
        {
            var createDate = DateTimeOffset.Parse(metadata.CreateDate.ToString());
            var name = content.ReferenceName.ToString();
            var id = content.ReferenceId.ToString();
            var frequency = content.Frequency.ToString();
            var repeatIfAllClosed = bool.Parse(content.RepeatIfAllClosed?.ToString() ?? "false");

            Repeat.Add(new RepeatItem
            {
                Id = Guid.NewGuid().ToString(),
                ReferenceName = name,
                ReferenceId = id,
                Frequency = frequency,
                RepeatIfAllClosed = repeatIfAllClosed,
                StartTime = createDate,
                LastGeneratedTime = createDate
            });
        }

        internal static void Reset(dynamic a, dynamic b) => Repeat = new List<RepeatItem>();

        public static List<RepeatItem> Repeat = new List<RepeatItem>();

        public class RepeatItem
        {
            public string Id { get; set; }
            public string ReferenceName { get; set; }
            public string ReferenceId { get; set; }
            public bool RepeatIfAllClosed { get; set; }

            private string frequency;

            public string Frequency
            {
                get => frequency;
                set
                {
                    frequency = value;
                    FrequencyType = frequency.Substring(0, 1);
                    int.TryParse(frequency.Substring(1), out int n);
                    FrequencyNumber = n;
                }
            }

            public string FrequencyType { get; internal set; }
            public int FrequencyNumber { get; internal set; }
            public DateTimeOffset StartTime { get; set; }
            public DateTimeOffset EndTime { get; set; }
            public DateTimeOffset LastGeneratedTime { get; set; }

            public DateTimeOffset NextGeneratedTime
            {
                get
                {
                    var result = DateTimeOffset.MaxValue;
                    var lastRun = LastGeneratedTime;
                    if (FrequencyType == "T") // test repeat every run
                    {
                        result = lastRun;
                    }
                    else if (FrequencyType == "H")
                    {
                        result = lastRun.AddHours(FrequencyNumber);
                    }
                    else if (FrequencyType == "D")
                    {
                        result = lastRun.AddDays(FrequencyNumber);
                    }
                    else if (FrequencyType == "W")
                    {
                        result = lastRun.AddDays(FrequencyNumber * 7);
                    }
                    else if (FrequencyType == "M")
                    {
                        result = lastRun.AddMonths(FrequencyNumber);
                    }
                    else if (FrequencyType == "Y")
                    {
                        result = lastRun.AddYears(FrequencyNumber);
                    }
                    return result;
                }
            }
        }
    }
}