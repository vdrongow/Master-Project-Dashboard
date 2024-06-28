using System;
using UnityEngine;

namespace Adlete
{
    /// <summary>
    /// Use the ModuleConnection class to connect your client application to an adlete-service.
    /// The ModuleConnection acts a API-Wrapper and holds a list of all request which can be 
    /// processed by the adlete-service.
    /// </summary>
    [Serializable]
    public class ModuleConnection : MonoBehaviour
    {
        [Tooltip("holds the connection URI to the service")]
        [SerializeField]
        public string apiUrl = "http://localhost:5000/graphql";

        [Tooltip("holds the information about a client token which is used by request to authenticate the client")]
        [SerializeField]
        public string clientToken = "";

        [Tooltip("holds a learnerId prefix which is used during creating new learners on the service")]
        [SerializeField]
        public string newLearnerIdPrefix;

        [Tooltip("holds a list of activities which are supported by this client. Make sure this list is a subset of the given list from serviceConfiguration.")]
        [SerializeField]
        private string[] recommendedActivityList;

        [Tooltip("holds a instance of a GraphQLClient to communicate with a graphql service")]
        private GraphQLClient client;

        [Tooltip("holds the learnerId of the currently loggedIn learner. Requests will use this Id.")]
        private string loggedInLearnerId;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            client = new GraphQLClient(apiUrl);
        }

        #region Service Routines
        /// <summary>
        /// Checks the server status, and returns also information about the authorization status.
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void CheckStatus(Action<StatusInfo> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(ServiceRoutines.CheckStatus(this.client, this.clientToken, successCallback, errorCallback, finallyCallback));
        }

        /// <summary>
        /// Fetching the service configuration
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void FetchServiceConfiguration(Action<ServiceConfiguration> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(ServiceRoutines.FetchServiceConfiguration(this.client, this.clientToken, successCallback, errorCallback, finallyCallback));
        }

        #endregion

        #region Learner Routines    
        /// <summary>
        /// Login a user with a given learnerId 
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="initialScalarBeliefSetId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void Login(string learnerId, Action<Learner> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.FetchLearner(learnerId,
            (learner) =>
            {
                this.loggedInLearnerId = learnerId;
                successCallback(learner);
            },
            errorCallback,
            finallyCallback);
        }

        /// <summary>
        /// Fetching the logged in learner. Use the method Login to log a 
        /// user in with a given learner ID.
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void FetchLearner(Action<Learner> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(LearnerRoutines.FetchLearner(this.client, this.clientToken, this.loggedInLearnerId, successCallback, errorCallback, finallyCallback));
        }

        /// <summary>
        /// Creating a learner on server side. A new learner with a unique learnerId starting with the given prefix will be generated.
        /// If the creation was successful the created user is logged-in.
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        public void CreateNewLearnerAndLogin(string initialScalarBeliefSetId, Action<Learner> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            string learnerId = this.newLearnerIdPrefix + "_" + ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds();
            this.CreateLearner(learnerId, initialScalarBeliefSetId,
            (learner) =>
            {
                this.loggedInLearnerId = learnerId;
                successCallback(learner);
            },
            errorCallback,
            finallyCallback);
        }

        /// <summary>
        /// Use Logout to log out a learner
        /// </summary>
        public void Logout(Action successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.loggedInLearnerId = "";
            successCallback();

            finallyCallback();
        }

        /// <summary>
        /// Use LogoutAndDeleteLearner to log out a learner and completely remove the lenaers history from the service
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void LogoutAndDeleteLearner(Action<StatusInfo> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.DeleteLearner(this.loggedInLearnerId, successCallback, errorCallback, finallyCallback);
        }

        /// <summary>
        /// Create a learner with a given learnerId
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="initialScalarBeliefSetId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void CreateLearner(string learnerId, string initialScalarBeliefSetId, Action<Learner> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(LearnerRoutines.CreateLearner(this.client, this.clientToken, learnerId, initialScalarBeliefSetId, successCallback, errorCallback, finallyCallback));
        }

        /// <summary>
        /// Fetch a learner with a given learnerId
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void FetchLearner(string learnerId, Action<Learner> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(LearnerRoutines.FetchLearner(this.client, this.clientToken, learnerId, successCallback, errorCallback, finallyCallback));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        public void DeleteLearner(string learnerId, Action<StatusInfo> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(LearnerRoutines.DeleteLearner(this.client, this.clientToken, learnerId, successCallback, errorCallback, finallyCallback));
        }
        #endregion

        #region Analysis Routines
        /// <summary>
        /// returns all collected data from the logged-in learner to analysis the learner´s performance
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void LearnerAnalytics(Action<LearnerAnalyticsData> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.LearnerAnalytics(this.loggedInLearnerId, successCallback, errorCallback, finallyCallback);
        }

        /// <summary>
        /// returns all collected data from the learner with the given learnerId. 
        /// You can use this data to analysis the learner´s performance.
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void LearnerAnalytics(string learnerId, Action<LearnerAnalyticsData> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(AnalysisRoutines.fetchLearnerAnalytics(this.client, this.clientToken, learnerId, successCallback, errorCallback, finallyCallback));
        }

        //ToDo simulateLearner 
        #endregion

        #region Activity Routines
        /// <summary>
        /// Submit an activity result created by the logged in user
        /// </summary>
        /// <param name="observation"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void SubmitActivityResult(Observation observation, Action<StatusInfo> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.SubmitActivityResult(this.loggedInLearnerId, observation, successCallback, errorCallback, finallyCallback);
        }

        /// <summary>
        /// Submit an activity result created by the learner with the given learnerId
        /// </summary>
        /// <param name="observation"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void SubmitActivityResult(string learnerId, Observation observation, Action<StatusInfo> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(ActivityRoutines.SubmitActivityResult(this.client, this.clientToken, learnerId, observation, successCallback, errorCallback, finallyCallback));
        }

        /// <summary>
        /// Fetch the next recommended activity and its estimated difficulty value for the logged-in user.
        /// If your client does not support activities for all activity-types update the recommendedActivityList.
        /// You can fetch the list of activity-types by using the ServiceConfiguration routine.
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void FetchNextActivityRecommendation(Action<Recommendation> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.FetchNextActivityRecommendation(this.loggedInLearnerId, successCallback, errorCallback, finallyCallback);
        }

        /// <summary>
        /// Fetch the next recommended activity and its estimated difficulty value for the learner with the given learnerId
        /// If your client does not support activities for all activity-types update the recommendedActivityList.
        /// You can fetch the list of activity-types by using the ServiceConfiguration routine.
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void FetchNextActivityRecommendation(string learnerId, Action<Recommendation> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(ActivityRoutines.FetchNextActivityRecommendation(this.client, this.clientToken, learnerId, this.recommendedActivityList, successCallback, errorCallback, finallyCallback));
        }
        #endregion

        #region Session Routines
        /// <summary>
        /// Start a session for the logged in user.
        /// The start session routine starts a session on the service
        /// A session will be open as long as a client close it again.
        /// Sessions should help you to structure your training units.
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void StartSession(Action<Session> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.StartSession(this.loggedInLearnerId, successCallback, errorCallback, finallyCallback);
        }
        /// <summary>
        /// Start a session for the user with the given learnerId.
        /// The start session routine starts a session on the service
        /// A session will be open as long as a client close it again.
        /// Sessions should help you to structure your training units.
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void StartSession(string learnerId, Action<Session> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(SessionRoutines.StartSession(this.client, this.clientToken, learnerId, successCallback, errorCallback, finallyCallback));
        }

        /// <summary>
        /// Fetch the current active session of the currently logged-in user
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void FetchActiveSession(Action<Session> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.FetchActiveSession(this.loggedInLearnerId, successCallback, errorCallback, finallyCallback);
        }

        /// <summary>
        /// Fetch the current active session of the user with the given learnerId
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void FetchActiveSession(string learnerId, Action<Session> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(SessionRoutines.FetchActiveSession(this.client, this.clientToken, learnerId, successCallback, errorCallback, finallyCallback));
        }

        /// <summary>
        /// Stop the current active session of the logged-in user
        /// </summary>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void StopSession(Action<StatusInfo> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            this.StopSession(this.loggedInLearnerId, successCallback, errorCallback, finallyCallback);
        }

        /// <summary>
        /// Stop the current active session of the learner with the given learnerId.
        /// </summary>
        /// <param name="learnerId"></param>
        /// <param name="successCallback"></param>
        /// <param name="errorCallback"></param>
        /// <param name="finallyCallback"></param>
        public void StopSession(string learnerId, Action<StatusInfo> successCallback = null, Action<string> errorCallback = null, Action finallyCallback = null)
        {
            StartCoroutine(SessionRoutines.StopSession(this.client, this.clientToken, learnerId, successCallback, errorCallback, finallyCallback));
        }
        #endregion
    }
}
