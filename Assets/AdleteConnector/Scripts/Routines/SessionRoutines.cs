using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    /// <summary>
    /// The class SessionRoutines contains a list of session related routines
    /// Use sessions to structure your trainings units. The recommendation
    /// system uses the history which is stored in a session object.
    /// Session won't get closed until a client requests the service to do so.
    /// Therefore its up to you to open and close sessions.
    /// </summary>
    public class SessionRoutines
    {
        /// <summary>
        /// The start session routine starts a session on the service
        /// A session will be open as long as a client close it again.
        /// Sessions should help you to structure your training units.
        /// </summary>
        /// <param name="clientToken"></param>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        public static IEnumerator StartSession(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            Action<Session> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"mutation startSession($learnerId:String!){
                                startSession(learnerId:$learnerId){
                                    activitiesStatistics {
                                        activityName, 
                                        activitiesPlayed, 
                                        sumAnswersCorrectness, 
                                        avgAnswersCorrectness
                                    }, 
                                    startTimestamp, 
                                    stopTimestamp,
                                    active,
                                }
                            }";
            string variables = $"{{ \"learnerId\": \"{learnerId}\" }}";

            using (UnityWebRequest request = client.Query(query, variables, clientToken, "startSession"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["startSession"];

                        successCallback?.Invoke(Session.Deserialize(jsonData));
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

        public static IEnumerator FetchActiveSession(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            Action<Session> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"query activeSession($learnerId: String!) {
                                            activeSession(learnerId: $learnerId) {
                                                    startTimestamp
                                                    stopTimestamp
                                                    active
                                                    activitiesStatistics {
                                                        activityName
                                                        activitiesPlayed
                                                        avgAnswersCorrectness
                                                        sumAnswersCorrectness
                                                    }
                                            }
                                        }";

            string variables = $"{{ \"learnerId\": \"{learnerId}\" }}";

            using (UnityWebRequest request = client.Query(query, variables, clientToken, "activeSession"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["activeSession"];

                        successCallback?.Invoke(Session.Deserialize(jsonData));
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
        /// Use the StopSession method to close an open session.
        /// </summary>
        /// <param name="clientToken"></param>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <returns></returns>
        public static IEnumerator StopSession(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            Action<StatusInfo> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"mutation stopSession($learnerId:String!) {
                                stopSession(learnerId:$learnerId) {
                                    statusCode, 
                                    statusDescription,
                                    timestamp
                                }
                            }";
            string variables = $"{{ \"learnerId\": \"{learnerId}\" }}";

            using (UnityWebRequest request = client.Query(query, variables, clientToken, "stopSession"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["stopSession"];

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
            }

            finallyCallback?.Invoke();
        }

    }
}