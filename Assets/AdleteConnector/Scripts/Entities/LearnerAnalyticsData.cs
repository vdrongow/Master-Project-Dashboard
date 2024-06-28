using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    [Serializable]
    public class LearnerAnalyticsData
    {
        public ServiceConfiguration serviceConfiguration;
        public Learner learner;
        public List<Observation> observations;

        public static LearnerAnalyticsData Deserialize(JToken jsonData)
        {
            LearnerAnalyticsData learnerAnalytics = new LearnerAnalyticsData();

            learnerAnalytics.serviceConfiguration = ServiceConfiguration.Deserialize(jsonData["serviceConfiguration"]);
            learnerAnalytics.learner = Learner.Deserialize(jsonData["learner"]);

            learnerAnalytics.observations = new List<Observation>();
            foreach (JToken observation in jsonData.SelectToken("observations"))
            {
                learnerAnalytics.observations.Add(Observation.Deserialize(observation));
            }

            return learnerAnalytics;
        }
    }
}
