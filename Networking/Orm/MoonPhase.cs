using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace SharpKnP321.Networking.Orm
{
    // ORM for daily data
    internal class MoonPhase
    {
        [JsonPropertyName("phaseName")]
        public String PhaseName { get; set; } = null!;

        [JsonPropertyName("isPhaseLimit")]
        public dynamic IsPhaseLimit { get; set; } = null!;
        
        [JsonPropertyName("lighting")]
        public double Lighting { get; set; }
        
        [JsonPropertyName("svg")]
        public String Svg { get; set; } = null!;

        [JsonPropertyName("svgMini")]
        public dynamic SvgMini { get; set; } = null!;

        [JsonPropertyName("timeEvent")]
        public dynamic TimeEvent { get; set; } = null!;

        [JsonPropertyName("dis")]
        public double Distance { get; set; }

        [JsonPropertyName("dayWeek")]
        public int DayWeek { get; set; }

        [JsonPropertyName("npWidget")]
        public String NpWidget { get; set; } = null!;
    }
}
/*
"phaseName": "Waning",
"isPhaseLimit": false,
"lighting": 97.06347458969742,
"svg": "<svg width=\"150\" height=\"150\" viewBox=\"0 0 100 100\"><defs><pattern id=\"image11\" x=\"0\" y=\"0\" patternUnits=\"userSpaceOnUse\" height=\"100\" width=\"100\"><image x=\"0\" y=\"0\" height=\"100\" width=\"100\" xlink:href=\"https://www.icalendar37.net/lunar/api/i.png\"></image></pattern></defs><g><circle cx=\"50\" cy=\"50\" r=\"49\" stroke=\"none\"  fill=\"gray\"/><path d=\"M 50 1 A 49,49 0 1,0 49,99 A -46.06,49 0 1,0 50,1\" stroke-width=\"0\" stroke=\"none\" fill=\"#FFFF88\" /><a xlink:href=\"https://www.icalendar37.net/lunar/app/\" rel=\"noopener noreferrer\" target=\"_blank\"><circle cx=\"50\" cy=\"50\" r=\"49\" style=\"pointer-events:all;cursor:pointer\" stroke-width=\"0\"   fill=\"url(#image11)\" /></a></g></svg>",
"svgMini": false,
"timeEvent": false,
"dis": 380322.97672234825,
"dayWeek": 1,
"npWidget": "Waning (97%)",
 */