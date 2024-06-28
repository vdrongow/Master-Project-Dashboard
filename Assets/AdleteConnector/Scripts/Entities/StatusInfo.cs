using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    [Serializable]
    public class StatusInfo
    {
        public int statusCode;
        public string statusDescription;
        public DateTime timestamp;

        public static StatusInfo Deserialize(JToken jsonData)
        {
            return JsonConvert.DeserializeObject<StatusInfo>(jsonData.ToString(), GraphQLClient.GetJsonDateFormatSerializerSettings());
        }
    }
}
