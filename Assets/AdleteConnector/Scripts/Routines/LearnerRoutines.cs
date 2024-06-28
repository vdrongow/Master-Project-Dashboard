using System;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    /// <summary>
    /// The class LearnerRoutines contains a list of learner related routines
    /// </summary>
    public class LearnerRoutines
    {
        /// <summary>
        /// Use CreateLearner to create a new learner account on server side.
        /// The new account will be initialed with the given initialScalarBeliefSet.
        /// You can fetch the list of initialScalarBeliefSets by using ServiceConfiguration routine.
        /// The selected initialScalarBeliefSet is the initial competence estimation of the learner,
        /// it helps the system to calibrate faster. 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientToken"></param>
        /// <param name="learnerId"></param>
        /// <param name="initialScalarBeliefSetId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        /// <returns></returns>
        public static IEnumerator CreateLearner(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            string initialScalarBeliefSetId,
            Action<Learner> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"mutation createLearner($learnerId:String!, $initialScalarBeliefSetId:String!) {
                                                createLearner(learnerId:$learnerId, initialScalarBeliefSetId:$initialScalarBeliefSetId) {
                                                    learnerId, 
                                                    probabilisticBeliefs, 
                                                    scalarBeliefs, 
                                                    tendencies, 
                                                    overAllFinishedActivities
                                                }
                                            }";

            string variables = $"{{ \"learnerId\": \"{learnerId}\", \"initialScalarBeliefSetId\": \"{initialScalarBeliefSetId}\" }}";

            using (UnityWebRequest request = client.Query(query, variables, clientToken, "createLearner"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["createLearner"];

                        successCallback?.Invoke(Learner.Deserialize(jsonData));
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
        /// FetchLearner loads the learner data of the learner with the given learnerId.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientToken"></param>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        /// <returns></returns>
        public static IEnumerator FetchLearner(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            Action<Learner> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"query learner($learnerId:String!){
                                learner(learnerId:$learnerId){
                                    learnerId, 
                                    scalarBeliefs, 
                                    probabilisticBeliefs, 
                                    tendencies, 
                                    overAllFinishedActivities
                                }
                            }";
            string variables = $"{{ \"learnerId\": \"{learnerId}\" }}";

            using (UnityWebRequest request = client.Query(query, variables, clientToken, "learner"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken jsonData = JObject.Parse(responseString).GetValue("data")["learner"];

                        successCallback?.Invoke(Learner.Deserialize(jsonData));
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
        /// Use DeleteLearner to delete a learner account completely from the server.
        /// Beware that this removes also the learners history.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientToken"></param>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        /// <returns></returns>
        public static IEnumerator DeleteLearner(
            GraphQLClient client,
            string clientToken,
            string learnerId,
            Action<StatusInfo> successCallback = null,
            Action<string> errorCallback = null,
            Action finallyCallback = null)
        {
            string query = @"mutation deleteLearner($learnerId:String!){
                                deleteLearner(learnerId:$learnerId){
                                    statusCode, 
                                    statusDescription, 
                                    timestamp
                                }
                            }";
            string variables = $"{{ \"learnerId\": \"{learnerId}\" }}";

            using (UnityWebRequest request = client.Query(query, variables, clientToken, "deleteLearner"))
            {
                yield return request.SendWebRequest();

                if (GraphQLClient.VerifyResponse(request))
                {
                    string responseString = request.downloadHandler.text;
                    bool isError = GraphQLClient.IsResponseError(responseString);
                    if (!isError)
                    {
                        JToken response = JObject.Parse(responseString).GetValue("data")["deleteLearner"];

                        successCallback?.Invoke(StatusInfo.Deserialize(response));
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
    }
}