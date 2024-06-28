using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    /// <summary>
    /// The class ServiceRoutines contains a list of service related routines
    /// checkStatus
    /// </summary>
    public class ServiceRoutines
    {
        /// <summary>
        /// Checks the server status, and returns also information about the authorization status.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientToken"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        /// <returns></returns>
        public static IEnumerator CheckStatus(
            GraphQLClient client,
            string clientToken,
            Action<StatusInfo> successCallback,
            Action<string> errorCallback,
            Action finallyCallback = null)
        {
            string query = @"query status {
                                        status {
                                            statusCode,
                                            statusDescription,
                                            timestamp
                                        }
                            }";

            using (UnityWebRequest request = client.Query(query, "", clientToken, "status"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {

                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["status"];

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
        /// Fetching the service configuration and returns
        /// the set of defined activities and a set of initialScalarBeliefIds
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientToken"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        /// <returns></returns>
        public static IEnumerator FetchServiceConfiguration(
            GraphQLClient client,
            string clientToken,
            Action<ServiceConfiguration> successCallback,
            Action<string> errorCallback,
            Action finallyCallback = null)
        {
            string query = @"query serviceConfiguration {
                                        serviceConfiguration {
                                            activityNames, 
                                            initialScalarBeliefs
                                        }
                            }";

            using (UnityWebRequest request = client.Query(query, "", clientToken, "status"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    if (!GraphQLClient.IsResponseError(responseString))
                    {

                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["serviceConfiguration"];

                        successCallback?.Invoke(ServiceConfiguration.Deserialize(jsonData));
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
    }
}