using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Extensions
{
    public static class TransformExtensions
    {
        public static Transform FindMainParent(this Transform transform)
        {
            Transform mainParent = transform;

            while (mainParent.parent != null)
            {
                mainParent = mainParent.parent;
            }

            return mainParent;
        }
        
        public static RectTransform FindParentRectTransform(this Transform currentTransform)
        {
            Transform parent = currentTransform.parent;

            while (parent != null)
            {
                RectTransform parentRectTransform = parent.GetComponent<RectTransform>();

                if (parentRectTransform != null)
                {
                    return parentRectTransform;
                }

                parent = parent.parent;
            }

            return null;
        }

        public static void ScaleAnim(this Transform transform, float duration, Vector2 start, Vector2 end, UnityAction onComplete = null)
        {
            transform.localScale = Vector2.Lerp(start, end, duration / Time.deltaTime);
            onComplete?.Invoke();
        }
    }
}