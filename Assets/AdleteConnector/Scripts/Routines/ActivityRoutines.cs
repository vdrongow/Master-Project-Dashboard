using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Adlete
{
    /// <summary>
    /// The class ActivityRoutines contains all routines dealing with activities
    /// Use this routines within your main game loop: 
    ///     Submit results --> Receive recommendations for the next best fitting activity -->
    ///     Submit new results from recommended activity --> and so on
    /// </summary>
    public class ActivityRoutines
    {
        /// <summary>
        /// Submit an activity result which was created by the learner with the given learnerId
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientToken"></param>
        /// <param name="observation"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        public static IEnumerator SubmitActivityResult(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            Observation observation,
            Action<StatusInfo> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"mutation submitActivityResult($observation:ObservationInput!) {
                                                submitActivityResult(observation:$observation) {
                                                    statusCode, 
                                                    statusDescription, 
                                                    timestamp
                                                }
                                            }";


            string variables = "{\"observation\": " +
                        JsonConvert.SerializeObject(
                            new ObservationInput(observation, learnerId),
                            GraphQLClient.GetJsonDateFormatSerializerSettings()
                        ) +
                    "}";

            using (UnityWebRequest request = client.Query(query, variables, clientToken, "submitActivityResult"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["submitActivityResult"];

                        successCallback?.Invoke(StatusInfo.Deserialize(jsonData));
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

                finallyCallback?.Invoke();
            }
        }

        /// <summary>
        /// Fetch the next activity recommendation for a specific learner.
        /// If your client does not support activities for all activity-types add an activitySubset list to this call.
        /// You can fetch the list of activity-types by using the ServiceConfiguration routine.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientToken"></param>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        /// <returns></returns>
        public static IEnumerator FetchNextActivityRecommendation(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            string[] activitySubset,
            Action<Recommendation> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"mutation fetchNextRecommendation($learnerId: String!, $activitySubset: [String!]) {
                                                    fetchNextRecommendation(learnerId: $learnerId, activitySubset: $activitySubset) {
                                                                    activityName
                                                                    difficulty
                                                    }
                                        }";

            string variables;
            if (activitySubset == null || activitySubset.Length == 0)
            {
                variables = $"{{ \"learnerId\": \"{learnerId}\" }}";
            }
            else
            {
                variables = $"{{ \"learnerId\": \"{learnerId}\",\"activitySubset\": {JsonConvert.SerializeObject(activitySubset)}}}";
            }
            Debug.Log(variables);
            using (UnityWebRequest request = client.Query(query, variables, clientToken, "fetchNextRecommendation"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["fetchNextRecommendation"];

                        successCallback?.Invoke(Recommendation.Deserialize(jsonData));
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
        }

        /// <summary>
        /// Helper class to mimic observation input class of the submit activity result routines
        /// </summary>
        private class ObservationInput : Observation
        {
            public string learnerId;
            public ObservationInput(Observation observation, string learnerId)
            {
                this.activityName = observation.activityName;
                this.activityCorrectness = observation.activityCorrectness;
                this.activityDifficulty = observation.activityDifficulty;
                this.timestamp = observation.timestamp;
                this.learnerId = learnerId;
            }

        }
    }
}