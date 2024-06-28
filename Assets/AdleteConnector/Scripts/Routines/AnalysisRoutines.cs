using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    /// <summary>
    /// The class AnalysisRoutines holds wrapper functions to access service analysis routines 
    /// Use this class to visualizer the learner progress or providing statistics about the progress
    /// </summary>
    public class AnalysisRoutines
    {
        /// <summary>
        /// Returns collected data from the given learner to analysis the learner´s performance
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientToken"></param>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        /// <returns></returns>
        public static IEnumerator fetchLearnerAnalytics(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            Action<LearnerAnalyticsData> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"query learnerAnalytics($learnerId: String!) {
                learnerAnalytics(learnerId: $learnerId) {
                    serviceConfiguration {
                        activityNames,
                        initialScalarBeliefs
                    }
                    learner {
                        learnerId
                        probabilisticBeliefs
                        scalarBeliefs
                        tendencies
                        overAllFinishedActivities
                    }
                    observations {
                        activityName
                        activityDifficulty
                        activityCorrectness
                        timestamp
                    }
                }
            }";
            string variables = $"{{ \"learnerId\": \"{learnerId}\" }}";

            using (UnityWebRequest request = client.Query(query, variables, clientToken, "learnerAnalytics"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["learnerAnalytics"];

                        successCallback?.Invoke(LearnerAnalyticsData.Deserialize(jsonData));
                    }
                    else
                    {
                        errorCallback?.Invoke(GraphQLClient.GetErrorMessage(responseString));
                    }
                }
                else
                {
                    errorCallback?.Invoke(request.error);
                }
            }

            finallyCallback?.Invoke();
        }
    }
}