using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    [Serializable]
    public class Observation
    {
        public string activityName;
        public float activityCorrectness;
        public float activityDifficulty;
        public DateTime timestamp;


        public static Observation Deserialize(JToken jsonData)
        {
            return JsonConvert.DeserializeObject<Observation>(jsonData.ToString(), GraphQLClient.GetJsonDateFormatSerializerSettings());
        }
    }
}