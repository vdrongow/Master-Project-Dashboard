using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    [Serializable]
    public class Recommendation
    {
        public string activityName;
        public float difficulty;

        public static Recommendation Deserialize(JToken jsonData)
        {
            return JsonConvert.DeserializeObject<Recommendation>(jsonData.ToString());
        }
    }
}
