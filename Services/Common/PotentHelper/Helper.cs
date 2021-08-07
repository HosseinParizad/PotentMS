using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PotentHelper
{
    public class Helper
    {
        public static dynamic GetMetadataByGroupKey(string groupKey)
            => new { GroupKey = groupKey, ReferenceKey = Guid.NewGuid().ToString() };

        public static dynamic Deserialize(string content)
            => JsonConvert.DeserializeObject<dynamic>(content
                , new IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" });


        public static T DeserializeObject<T>(string content)
            => JsonConvert.DeserializeObject<T>(content
                , new IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" });

    }
}
