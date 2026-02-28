using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SharpKnP321.Networking.Orm
{
    internal class NbuRate
    {
        public int      R030         { get; set; }
        public String   Txt          { get; set; } = null!;
        public double   Rate         { get; set; }
        public String   Cc           { get; set; } = null!;
        public String?  Special      { get; set; }
        public DateOnly Exchangedate { get; set; }

        public static NbuRate FromJson(JsonElement jsonElement)
        {
            return new()
            {
                R030 = jsonElement.GetProperty("r030").GetInt32(),
                Txt = jsonElement.GetProperty("txt").GetString()!,
                Rate = jsonElement.GetProperty("rate").GetDouble(),
                Cc = jsonElement.GetProperty("cc").GetString()!,
                Special = jsonElement.GetProperty("special").GetString(),
                Exchangedate = DateOnly.Parse(jsonElement.GetProperty("exchangedate").GetString()!),
            };
        }

        public override string ToString()
        {
            return $"r030={R030}, txt={Txt}, rate={Rate}, cc={Cc}, Exchangedate={Exchangedate}";
        }
    }
}
/*  https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json
    "r030": 12,
    "txt": "Алжирський динар",
    "rate": 0.33331,
    "cc": "DZD",
    "exchangedate": "18.02.2026",
    "special": null | "N"
*/