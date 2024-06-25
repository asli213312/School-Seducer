using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using _School_Seducer_.Editor.Scripts.Services;

namespace _School_Seducer_.Editor.Scripts.Services
{
    public class ServerHelper : MonoBehaviour
    {
        public delegate void ResponseCallback(string response);

        public void GET(string uri) => StartCoroutine(GetRequest(uri));
        
        IEnumerator GetRequest(string uri)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(uri);
            yield return webRequest.SendWebRequest();
                
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                Debug.Log(webRequest.downloadHandler.text);
            }
        }

        public void POST(string uri, string jsonData, ResponseCallback callback) => StartCoroutine(PostRequest(uri, jsonData, callback));
        
        IEnumerator PostRequest(string uri, string jsonData, ResponseCallback callback)
        {
            using UnityWebRequest webRequest = new UnityWebRequest(uri, "POST");
            {
                byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                
                yield return webRequest.SendWebRequest();
                
                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error: " + webRequest.error);
                    callback?.Invoke(null);
                }
                else
                {
                    string response = webRequest.downloadHandler.text;
                    Debug.Log("Response: " + response);
                    callback?.Invoke(response);
                }
            }
        }
    }
}