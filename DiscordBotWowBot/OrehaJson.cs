using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotWowBot
{
    public struct OrehaJson
    {
        [JsonProperty("white")]
        public string White { get; set; }
        [JsonProperty("green")]
        public string Green { get; set; }
        [JsonProperty("blue")]
        public string Blue { get; set; }
        [JsonProperty("basic")]
        public string Basic { get; set; }
        [JsonProperty("superior")]
        public string Superior { get; set; }
        [JsonProperty("prime")]
        public string Prime { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
    }
}
