using System;
using System.Collections;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Extensions
{
    public static class MonoBehaviourExtensions
    {
	    private delegate IEnumerator CustomCoroutineDelegate();
	    
        public static Component FindComponentByInstanceID(this MonoBehaviour monoObject, int instanceID)
        {
            Component[] components = monoObject.GetComponents<Component>();
            
            return System.Array.Find(components, c => c.GetInstanceID() == instanceID);
        }

        public static void InvokeCoroutine(this MonoBehaviour monoObject, Func<IEnumerator> routineFunc)
        {
	        monoObject.StartCoroutine(routineFunc());
        }
        
        public static void WaitForSeconds(this MonoBehaviour monoObject, float seconds, Action onComplete = null)
        {
	        monoObject.StartCoroutine(WaitForSecondsProcess(seconds, onComplete));
        }

        public static void WaitUntil(this MonoBehaviour monoObject, bool condition, Action onComplete = null)
        {
	        monoObject.StartCoroutine(WaitUntilProcess(condition, onComplete));
        }

        private static IEnumerator WaitUntilProcess(bool condition, Action onComplete = null)
        {
	        yield return new WaitUntil(() => condition);
	        onComplete?.Invoke();
        }

        private static IEnumerator WaitForSecondsProcess(float seconds, Action onComplete = null)
        {
	        yield return new WaitForSeconds(seconds);
	        onComplete?.Invoke();
        }
        
        public static void Activate(this MonoBehaviour monoObject, Transform obj, float delay = 0)
        {
	        monoObject.StartCoroutine(ActivateProcess(obj.gameObject, delay));
        }

        public static void Deactivate(this MonoBehaviour monoObject, Transform obj, float delay = 0)
        {
	        monoObject.StartCoroutine(DeactivateProcess(obj.gameObject, delay));
        }

        private static IEnumerator ActivateProcess(GameObject obj, float delay = 0)
        {
	        if (obj.activeSelf) yield break;
	        
	        if (delay > 0)
	        {
		        yield return new WaitForSeconds(delay);
		        obj.SetActive(true);
	        }
	        else
		        obj.SetActive(true);
        }

        private static IEnumerator DeactivateProcess(GameObject obj, float delay = 0)
        {
	        if (!obj.activeSelf) yield break;
	        
	        if (delay > 0)
	        {
		        yield return new WaitForSeconds(delay);
		        obj.SetActive(false);
	        }
	        else
		        obj.SetActive(false);
        }
    }
}