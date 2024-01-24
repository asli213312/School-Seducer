using System;
using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Extensions;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    [Serializable]
    public class AnimationMove : AnimationBase
    {
        public float duration;
        public Vector3 newPosition;

        public override void Invoke()
        {
             Behavior.StartCoroutine(DoLocalMove(newPosition));
        }

        private IEnumerator DoLocalMove(Vector3 position)
        {
            if (Target.transform is RectTransform targetRect)
            {
                Vector2 startPosition  = targetRect.anchoredPosition;
                float elapsedTime = 0f;
                
                while (elapsedTime < duration)
                {
                    float t = Mathf.Clamp01(elapsedTime / duration);
                    targetRect.anchoredPosition = Vector2.Lerp(startPosition, position, t);
                    yield return null;
                    elapsedTime += Time.deltaTime;
                }

                targetRect.anchoredPosition = position;   
            }
            else
            {
                Vector2 startPosition = Target.transform.position;
                float elapsedTime = 0f;

                while (elapsedTime < duration)
                {
                    float t = Mathf.Clamp01(elapsedTime / duration);
                    Target.transform.position = Vector2.Lerp(startPosition, position, t);
                    yield return null;
                    elapsedTime += Time.deltaTime;
                }

                Target.transform.position = position;   
            }
        }
    }
}