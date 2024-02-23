using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    public class NutakuManager : MonoBehaviour
    {
        public MyServer Server { get; private set; }
        
        private void Awake() 
        {
            GameObject serverGO = new GameObject("Server");
            Server = serverGO.AddComponent<MyServer>();
        }
        
       
        public void OnUserRetrieved(string userDataJson)
        {
            Debug.Log("User Retrieving is begun UNITY");
        
            var userData = JsonUtility.FromJson<UserData>(userDataJson);
            
                // Получаем необходимые поля из userData
                string userId = userData.userId;
                string token = userData.token;

                Debug.Log("Retrieved  userId in UNITY: " + userId);
                Debug.Log("Retrieved  token in UNITY: " + token);
            
            Server.CheckUserToken(userId, token, (isValidToken) => {
                if (isValidToken)
                {
                    Debug.Log("Token is valid. Sending request to server.");
                    // Если токен действителен, отправляем запрос на сервер
                    Server.MakeRequestToServerToSetUserToken(userId, token);
                }
                else
                {
                    Debug.Log("Token is invalid or expired. Need to refresh token.");
                    // Если токен недействителен или истек, обновляем его
                    RefreshToken(userId);
                }
            });
        }

        // Метод для обновления токена
        private void RefreshToken(string userId)
        {
            // Отправляем запрос на сервер для обновления токена
            Server.RefreshToken(userId, (newToken) => {
                Debug.Log("New token received: " + newToken);
                // После получения нового токена повторно отправляем запрос на сервер
                Server.MakeRequestToServerToSetUserToken(userId, newToken);
            });
        }
        
        [System.Serializable]
        private class UserData
        {
            public string userId;
            public string token;
        }
    }
}