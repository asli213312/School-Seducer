using System.Threading.Tasks;
using UnityEngine;

namespace _Kittens__Kitchen.Editor.Scripts.Utility.Extensions
{
    public static class GameObjectExtensions
    {
        public static void SafeDeactivate(this GameObject gameObject)
        {
            if (gameObject.activeSelf == false)
                Debug.LogWarning("GameObject not active for safe deactivate!");
            
            if (gameObject.transform.localScale == Vector3.one)
                gameObject.transform.localScale = Vector3.zero;
            else
                Debug.LogWarning("GameObject already safe inactive: ", gameObject);
        }
        
        public static void SafeActivate(this GameObject gameObject)
        {
            if (gameObject.activeSelf == false)
                Debug.LogWarning("GameObject not active for safe activate!");
            
            if (gameObject.transform.localScale == Vector3.zero)
                gameObject.transform.localScale = Vector3.one;
            else
                Debug.LogWarning("GameObject already safe active: ", gameObject);
        }
        
        public static void Activate(this GameObject gameObject)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            else
                Debug.LogWarning("GameObject already active: ", gameObject);
        }
        
        public static async void Activate(this GameObject gameObject, float delay = 0f)
        {
            if (!gameObject.activeSelf)
            {
                await Task.Delay((int)(delay * 1000));
                gameObject.SetActive(true);
            }
            else
                Debug.LogWarning("GameObject already active: ", gameObject);
        }
        
        public static void Deactivate(this GameObject gameObject)
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
            else
                Debug.LogWarning("GameObject already inactive: ", gameObject);
        }
        
        public static async void Deactivate(this GameObject gameObject, float delay = 0f)
        {
            if (gameObject.activeSelf)
            {
                await Task.Delay((int)(delay * 1000));
                gameObject.SetActive(false);
            }
            else
                Debug.LogWarning("GameObject already inactive: ", gameObject);
        }

        public static void Destroy(this GameObject gameObject, float delay = 0f)
        {
            if (gameObject != null)
                Object.Destroy(gameObject, delay);
            else
                Debug.LogWarning("GameObject already destroyed: " + gameObject.name);
        }
    }
}