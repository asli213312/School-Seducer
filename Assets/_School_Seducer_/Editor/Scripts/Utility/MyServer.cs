using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Nutaku.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class MyServer : MonoBehaviour
    {
        private const string USER_DATA_URL = "https://college-fuck-fest.nyc3.cdn.digitaloceanspaces.com/userData.json";
        
        public void MakeRequestToServerToSetUserToken(string id, string token)
        {
            // Создаем JSON объект для добавления нового токена
            JObject tokenObject = new JObject();
            tokenObject["id"] = id;
            tokenObject["token"] = token;

            // Получаем содержимое файла JSON
            StartCoroutine(GetJson(USER_DATA_URL, (jsonData) => {
                // Парсим содержимое JSON
                JObject json = JObject.Parse(jsonData);

                // Добавляем новый токен в JSON
                json["tokens"].Append(tokenObject);

                // Преобразуем обновленный JSON обратно в строку
                string updatedJsonData = json.ToString();

                // Отправляем запрос на сервер для обновления файла JSON
                StartCoroutine(SendJson(USER_DATA_URL, updatedJsonData, () => {
                    Debug.Log("User token successfully set on server.");
                }));
            }));
        }
        
        public void CheckUserToken(string userId, string token, Action<bool> callback)
        {
            StartCoroutine(GetJson(USER_DATA_URL, (jsonData) => {
                if (jsonData != null)
                {
                    // Парсим содержимое JSON
                    JObject json = JObject.Parse(jsonData);

                    // Находим токен пользователя по userId
                    JToken userToken = json["tokens"].FirstOrDefault(t => (string)t["id"] == userId);

                    if (userToken != null)
                    {
                        // Получаем текущий токен пользователя
                        string currentToken = (string)userToken["token"];
                
                        // Проверяем совпадение токенов
                        bool isValidToken = currentToken == token;

                        callback(isValidToken);
                    }
                    else
                    {
                        Debug.Log("User not found.");
                        callback(false);
                    }
                }
                else
                {
                    // Обработка ошибки получения данных из файла JSON
                    Debug.LogError("Failed to get JSON data.");
                    callback(false);
                }
            }));
        }

        public void RefreshToken(string userId, Action<string> tokenCallback)
        {
            // Получаем содержимое файла JSON
            StartCoroutine(GetJson(USER_DATA_URL, (jsonData) => {
                // Парсим содержимое JSON
                JObject json = JObject.Parse(jsonData);

                // Находим токен пользователя по userId
                JToken userToken = json["tokens"].FirstOrDefault(t => (string)t["id"] == userId);

                if (userToken != null)
                {
                    // Получаем текущий токен пользователя
                    string currentToken = (string)userToken["token"];
                    // Здесь можно добавить логику проверки срока действия токена

                    // Вызываем обратный вызов с текущим токеном
                    tokenCallback(currentToken);
                }
                else
                {
                    Debug.Log("User token not found.");
                    // Вызываем обратный вызов с пустым токеном, если пользователь не найден
                    tokenCallback("");
                }
            }));
        }
        
        private IEnumerator GetJson(string url, Action<string> callback)
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(url);
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                callback(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to get JSON data: " + webRequest.error);
                callback(null);
            }
        }

        private IEnumerator SendJson(string url, string jsonData, Action callback)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, "PUT"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    callback();
                }
                else
                {
                    Debug.LogError("Failed to send JSON data: " + webRequest.error);
                    callback();
                }
            }
        }
    }
}