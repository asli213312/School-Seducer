using System;
using System.Collections;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Extensions
{
    public static class CoroutineExtensions
    {
        public static void InlineWaitForSeconds(this MonoBehaviour monoObject, float seconds, Action onComplete = null)
        {
            monoObject.StartCoroutine(WaitForSecondsProcess(seconds, onComplete));
        }

        public static void InlineWaitUntil(this MonoBehaviour monoObject, bool condition, Action onComplete = null)
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
    }
}