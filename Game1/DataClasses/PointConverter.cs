using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1.DataClasses
{
    /// <summary>
    /// Inherits from JsonConverter class to deserialise JSON returned from graph database and convert to a Neo4j.Driver.Point object. 
    /// </summary>
    class PointConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Neo4j.Driver.Point));
        
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObj = JObject.Load(reader);
            int SrId = (int)jsonObj["SrId"];
            double X = (double)jsonObj["X"];
            double Y = (double)jsonObj["Y"];
            return new Neo4j.Driver.Point(SrId, X, Y);

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
