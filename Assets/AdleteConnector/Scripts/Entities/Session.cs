using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    [Serializable]
    public class Session
    {
        public List<ActivitiesStatistic> activitiesStatistics;
        public DateTime startTimestamp;
        public DateTime? stopTimestamp;
        public bool active;

        public static Session Deserialize(JToken jsonData)
        {
            Session session = new Session();

            session.startTimestamp = DateTime.Parse(jsonData.SelectToken("startTimestamp").ToString());
            if (jsonData.SelectToken("stopTimestamp").ToString() != "")
            {
                session.stopTimestamp = DateTime.Parse(jsonData.SelectToken("stopTimestamp").ToString());
            }

            session.activitiesStatistics = new List<ActivitiesStatistic>();
            foreach (JToken activitiesStatistic in jsonData.SelectToken("activitiesStatistics"))
            {
                session.activitiesStatistics.Add(ActivitiesStatistic.Deserialize(activitiesStatistic));
            }

            session.active = jsonData.SelectToken("active").Value<bool>();

            return session;
        }
    }
}
