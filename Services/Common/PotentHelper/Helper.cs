using System;
using System.Text.Json;

namespace PotentHelper
{
    public class Helper
    {
        public static dynamic GetMetadataByGroupKey(string groupKey)
            => new { GroupKey = groupKey, ReferenceKey = Guid.NewGuid().ToString() };

        public static dynamic Deserialize(string content)
            => JsonSerializer.Deserialize<dynamic>(content);
    }
}
