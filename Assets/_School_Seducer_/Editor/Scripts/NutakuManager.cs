using System.Linq.Expressions;
using System.Collections;
using System.Net;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using _School_Seducer_.Editor.Scripts.Services;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using Nutaku.Unity;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
	[System.Serializable]
    public class LoginParams
    {
        public string command;
        public Params @params;
    }

    [System.Serializable]
    public class Params
    {
        public string email;
        public string password;
    }

    [System.Serializable]
	public class LoginResponse
	{
	    public string code;
	    public Result result;
	    public string error_message;
	}

	[System.Serializable]
	public class Result
	{
	    public string autologin_token;
	    public string oauth_token;
	    public string oauth_token_secret;
	    public string user_id;
	    public string app_id;
	}

    public class NutakuManager : MonoBehaviour
    {
        [SerializeField] private ServerHelper server;

        [Header("DL_Client fields")]
        [SerializeField] private RectTransform bootScreen;
        [SerializeField] private RectTransform loginScreen;
        [SerializeField] private RectTransform loadingIndicator;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private Button loginButton;
        [SerializeField] private UnityEvent loginSuccessEvent;
        [SerializeField] private UnityEvent loginFailedEvent;

        private void Awake()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android: 
                    SdkPlugin.Initialize();
                    Debug.Log("User id: " + SdkPlugin.loginInfo.userId);
                    AndroidMakeRequest();
                    break;
                case RuntimePlatform.WindowsPlayer:
                	loginScreen.gameObject.SetActive(true);
                	loginButton.onClick.AddListener(OnLoginButtonClicked);
                	server.GET("https://fuckfest.college/scripts/authenticate.php");
                    break;
                case RuntimePlatform.WindowsEditor: // Добавлено для редактора
	                loginScreen.gameObject.SetActive(true);
	                loginButton.onClick.AddListener(OnLoginButtonClicked);
	                //server.GET("https://fuckfest.college/scripts/authenticate.php");
	                break;
            }
        }
        
        public void OnUserRetrieved(string userDataJson)
        {
            Debug.Log("User Retrieving is begun UNITY");
        }
        
        public void OnPaymentFinished(string response) 
        {
            Debug.Log("OnPaymentFinished is invoked in NutakuManager");
            Debug.Log("Result response: " + response);
        }

        private void OnLoginButtonClicked()
	    {
	        string email = emailField.text;
	        string password = passwordField.text;
	        
	        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
	        {
	            Debug.LogError("Email or Password is empty.");
	            return;
	        }

	        string jsonData = JsonUtility.ToJson(new LoginParams
	        {
	            command = "Login",
	            @params = new Params { email = email, password = password }
	        });

	        loadingIndicator.gameObject.SetActive(true);
	        StartCoroutine(LoginCoroutine(jsonData));
	    }

	    private IEnumerator LoginCoroutine(string jsonData)
		{
		    loadingIndicator.gameObject.SetActive(true);

		    // Создаем экземпляр UnityWebRequest для GET запроса
		    UnityWebRequest webRequest = UnityWebRequest.Get("https://sbox-mobileapi.nutaku.com/clientgame/");

		    // Отправляем GET запрос
		    yield return webRequest.SendWebRequest();

		    // Проверяем результат GET запроса
		    if (webRequest.result != UnityWebRequest.Result.Success)
		    {
		        Debug.LogError("Error during GET request: " + webRequest.error);
		        OnLoginFailed();
		        loadingIndicator.gameObject.SetActive(false);
		        yield break; // Выход из метода в случае неудачи
		    }

		    // Извлекаем куки из заголовка ответа
		    string cookieHeader = webRequest.GetRequestHeader("Set-Cookie");
		    if (!string.IsNullOrEmpty(cookieHeader))
		    {
		        // Разделяем куки по точке с запятой
		        string[] cookies = cookieHeader.Split(';');
		        foreach (string cookie in cookies)
		        {
		            // Добавляем каждую куку в UnityWebRequest для последующих запросов
		            Debug.Log("Cookie from Nutaku API: " + cookie);
		            webRequest.SetRequestHeader("Cookie", cookie);
		        }
		    }
		    else Debug.Log("Cookie is null");

		    // Теперь создаем экземпляр UnityWebRequest для POST запроса
		    webRequest = new UnityWebRequest("https://sbox-mobileapi.nutaku.com/clientgame/", "POST");

		    // Создаем экземпляр UploadHandler
		    byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
		    UploadHandlerRaw uploadHandlerRaw = new UploadHandlerRaw(bodyRaw);

		    // Устанавливаем UploadHandler
		    webRequest.uploadHandler = uploadHandlerRaw;

		    // Создаем экземпляр DownloadHandler
		    DownloadHandlerBuffer downloadHandlerBuffer = new DownloadHandlerBuffer();

		    // Устанавливаем DownloadHandler
		    webRequest.downloadHandler = downloadHandlerBuffer;

		    // Устанавливаем Content-Type заголовок
		    webRequest.SetRequestHeader("Content-Type", "application/json");

		    // Отправляем POST запрос
		    yield return webRequest.SendWebRequest();

		    // Проверяем результат POST запроса
		    if (webRequest.result != UnityWebRequest.Result.Success)
		    {
		        Debug.LogError("Error during POST request: " + webRequest.error);
		        OnLoginFailed();
		        loadingIndicator.gameObject.SetActive(false);
		        yield break; // Выход из метода в случае неудачи
		    }

		    string response = webRequest.downloadHandler.text;
		    Debug.Log("Response: " + response);

		    // Обработка ответа
		    var loginResponse = JsonUtility.FromJson<LoginResponse>(response);
		    Debug.Log("Login code: " + loginResponse.code);

		    if (loginResponse.code == "ok")
		    {
		        Debug.Log("Login successful! User ID: " + loginResponse.result.user_id);
		        OnLoginSuccess();
		    }
		    else
		    {
		        Debug.LogError("Login failed: " + loginResponse.error_message);
		        OnLoginFailed();
		    }

		    loadingIndicator.gameObject.SetActive(false);
		}

	    private void OnLoginResponse(string response)
	    {
	        if (string.IsNullOrEmpty(response))
	        {
	            Debug.LogError("Login failed: No response from server.");
	            return;
	        }

	        var loginResponse = JsonUtility.FromJson<LoginResponse>(response);
	        if (loginResponse.code == "ok")
	        {
	            Debug.Log("Login successful! User ID: " + loginResponse.result.user_id);
	            OnLoginSuccess();
	        }
	        else
	        {
	            Debug.LogError("Login failed: " + loginResponse.error_message);
	            OnLoginFailed();
	        }

	        loadingIndicator.gameObject.SetActive(false);
	    }

	    private void OnLoginSuccess() 
	    {
	    	loginScreen.transform.parent.gameObject.SetActive(false);
	    	bootScreen.gameObject.SetActive(true);

	    	loginSuccessEvent?.Invoke();
	    }

	    private void OnLoginFailed() 
	    {
	    	loginFailedEvent?.Invoke();
	    }

        private IEnumerator WindowsMakeRequest() 
        {
        	var url = "https://fuckfest.college/scripts/authenticate.php?id=" + SdkPlugin.loginInfo.userId;

        	yield return null;
        }

        private void AndroidMakeRequest()
        {
            var url = "https://fuckfest.college/scripts/authenticate.php?id=" + SdkPlugin.loginInfo.userId; // for initial testing, you can use https://postman-echo.com/post - this URL should echo your request back as a response
            var postData = new Dictionary<string, string>
            {
                { "id", SdkPlugin.loginInfo.userId }
            };

            try
            {
                RestApiHelper.PostMakeRequest(url, postData, this, TestBatchPostMakeRequestCallback);
            }
            catch (Exception ex)
            {
                // error handling
            }
 
            // callback method implementation:
            void TestBatchPostMakeRequestCallback(RawResult rawResult)
            {
                const string testName = "BatchPostMakeRequest";
                try
                {
                    if ((rawResult.statusCode > 199) && (rawResult.statusCode < 300))
                    {
                        var result = RestApi.HandlePostMakeRequestCallback(rawResult);
                        Debug.Log("Returned status code from the external server: " + result.rc);
                        // note that result.statusCode also exists, but this is the status code for your request to the Nutaku API server, not your destination server
 
                        Debug.Log("Returned response header(s) from the external server: ");
                        foreach (var header in result.headers)
                            Debug.LogFormat("\t{0}: {1}", header.Key, header.Value);
 
                        Debug.Log("Returned response body from the external server: \n" + Encoding.UTF8.GetString(rawResult.body));
                    }
                    else
                    {
                        Debug.LogError("Failed makeRequest android with body: " + Encoding.UTF8.GetString(rawResult.body) + " and rc: " + rawResult.statusCode);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Failed makeRequest android with body: " + Encoding.UTF8.GetString(rawResult.body) + " and rc: " + rawResult.statusCode + " and exception: " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        Debug.LogError("Failed makeRequest android with body: " + rawResult.body + " and rc: " +
                                       rawResult.statusCode + " and inner exception: " + ex.InnerException.Message);
                    }
                }
            }
        }
    }
}