using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Extensions
{
    public static class TransformExtensions
    {
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
    }
}