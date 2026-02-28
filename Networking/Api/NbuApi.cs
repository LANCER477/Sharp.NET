using SharpKnP321.Networking.Orm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SharpKnP321.Networking.Api
{
    internal class NbuApi
    {

        public static List<NbuRate> ListFromJsonString(String json)
        {
            return [..
                JsonSerializer.Deserialize<JsonElement>(json)
                .EnumerateArray()
                .Select(NbuRate.FromJson)
            ];
        }
    }
}
