using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Adlete
{
    public class GraphQLClient
    {
        private string url;

        public GraphQLClient(string url)
        {
            this.url = url;
        }

        public UnityWebRequest Query(string query, string variables, string accessToken, string operationName)
        {
            string json = BuildQuery(query, variables);

            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, UnityWebRequest.kHttpVerbPOST);

            if (!string.IsNullOrEmpty(accessToken))
                request.SetRequestHeader("client-token", accessToken);

            byte[] payload = Encoding.UTF8.GetBytes(json);
            UploadHandler data = new UploadHandlerRaw(payload);

            request.uploadHandler = data;
            request.SetRequestHeader("Content-Type", "application/json");

            return request;
        }

        private string BuildQuery(string query, string variables)
        {
            if (string.IsNullOrEmpty(variables))
            {
                return "{\"query\": \"" + TrimQuery(query) + "\"}"; 
            }
            else
            {
                return "{\"query\": \"" + TrimQuery(query) + "\", \"variables\": "+ variables + "}"; 
            }
        }

        private string TrimQuery(string query) {
            return query.Replace("\n","").Replace("\r","").Replace("\t","");
        }

        public static bool VerifyResponse(UnityWebRequest webRequest)
        {
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    return false;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.Log(webRequest.downloadHandler.text);
                    Debug.LogError(" HTTP Error: " + webRequest.error);
                    return false;
                case UnityWebRequest.Result.Success:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsResponseError(string response)
        {
            return response.Contains("error") || response.Contains("invalid");
        }

        public static string GetErrorMessage(string responseStr)
        {
            return JObject.Parse(responseStr).GetValue("errors")[0]["message"].ToString();
        }

        public static JsonSerializerSettings GetJsonDateFormatSerializerSettings()
        {
            JsonSerializerSettings dateFormatSettings = new JsonSerializerSettings();
            dateFormatSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";

            return dateFormatSettings;
        }

    }
}
