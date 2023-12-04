using System.Threading.Tasks;
using UnityEngine;

namespace _Kittens__Kitchen.Editor.Scripts.Utility.Extensions
{
    public static class GameObjectExtensions
    {
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