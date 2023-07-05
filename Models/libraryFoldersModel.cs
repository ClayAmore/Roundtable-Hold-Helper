using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roundtable.Models
{
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class _0
    {
        public string path { get; set; }
        public string label { get; set; }
        public string contentid { get; set; }
        public string totalsize { get; set; }
        public string update_clean_bytes_tally { get; set; }
        public string time_last_update_corruption { get; set; }
        public Apps apps { get; set; }
    }

    public class Apps
    {
        [JsonProperty("228980")]
        public string _228980 { get; set; }

        [JsonProperty("283640")]
        public string _283640 { get; set; }

        [JsonProperty("374320")]
        public string _374320 { get; set; }

        [JsonProperty("460950")]
        public string _460950 { get; set; }

        [JsonProperty("553420")]
        public string _553420 { get; set; }

        [JsonProperty("588650")]
        public string _588650 { get; set; }

        [JsonProperty("590380")]
        public string _590380 { get; set; }

        [JsonProperty("610370")]
        public string _610370 { get; set; }

        [JsonProperty("894020")]
        public string _894020 { get; set; }

        [JsonProperty("1145360")]
        public string _1145360 { get; set; }

        [JsonProperty("1147560")]
        public string _1147560 { get; set; }

        [JsonProperty("1245620")]
        public string _1245620 { get; set; }

        [JsonProperty("1369630")]
        public string _1369630 { get; set; }

        [JsonProperty("1432050")]
        public string _1432050 { get; set; }

        [JsonProperty("1740720")]
        public string _1740720 { get; set; }

        [JsonProperty("2369210")]
        public string _2369210 { get; set; }
    }

    public class Libraryfolders
    {
        [JsonProperty("0")]
        public _0 _0 { get; set; }
    }

    public class Root
    {
        public Libraryfolders libraryfolders { get; set; }
    }
}
