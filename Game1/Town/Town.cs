using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game1.Town.Districts;
using Newtonsoft.Json;

namespace Game1.Town
{
    public class Town
    {

        [JsonProperty("id")]
        public string id;

        public List<District> districts;

    }
}
