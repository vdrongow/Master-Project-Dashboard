using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Adlete
{
    [Serializable]
    public class ActivitiesStatistic
    {
        public string activityName;
        public float activitiesPlayed;
        public float sumAnswersCorrectness;
        public float avgAnswersCorrectness;

        public static ActivitiesStatistic Deserialize(JToken jsonData)
        {
            return JsonConvert.DeserializeObject<ActivitiesStatistic>(jsonData.ToString(), GraphQLClient.GetJsonDateFormatSerializerSettings());
        }
    }
}