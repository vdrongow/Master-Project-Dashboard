using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    [Serializable]
    public class ServiceConfiguration
    {
        public string[] activityNames;
        public string[] initialScalarBeliefIds;


        public static ServiceConfiguration Deserialize(JToken jsonData)
        {
            ServiceConfiguration serviceConfiguration = new ServiceConfiguration();


            serviceConfiguration.activityNames = JsonConvert.DeserializeObject<string[]>(jsonData.SelectToken("activityNames").ToString());

            JObject initialScalarBeliefs = JsonConvert.DeserializeObject<JObject>(jsonData.SelectToken("initialScalarBeliefs")?.ToObject<string>());
            serviceConfiguration.initialScalarBeliefIds = initialScalarBeliefs.Properties().Select(p => p.Name).ToArray();
            return serviceConfiguration;
        }
    }
}
