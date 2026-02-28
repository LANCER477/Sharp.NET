using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SharpKnP321.Networking.Orm
{
    internal class MoonApiResponse
    {
        [JsonPropertyName("phase")]
        public Dictionary<String, MoonPhase> Phase { get; set; } = [];
    }
}
