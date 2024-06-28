using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    [Serializable]
    public class Learner
    {
        public string learnerId;
        public List<string> probabilisticBeliefs;
        public List<string> scalarBeliefs;
        public List<string> tendencies;
        public int overAllFinishedActivities;

        public static Learner Deserialize(JToken jsonData)
        {
            Learner learner = new Learner();

            learner.learnerId = jsonData.SelectToken("learnerId").ToString();
            learner.overAllFinishedActivities = jsonData.SelectToken("overAllFinishedActivities").ToObject<int>();
            learner.probabilisticBeliefs = new List<string>(JsonConvert.DeserializeObject<string[]>(jsonData.SelectToken("probabilisticBeliefs").ToString()));
            learner.scalarBeliefs = new List<string>(JsonConvert.DeserializeObject<string[]>(jsonData.SelectToken("scalarBeliefs").ToString()));
            learner.tendencies = new List<string>(JsonConvert.DeserializeObject<string[]>(jsonData.SelectToken("scalarBeliefs").ToString()));

            return learner;
        }
    }
}
