using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class OffsetSizeDelta : MonoBehaviour
    {
        public float offsetHeight;
        public RectTransform rectToUpdate;
        public RectTransform rectOriginal;

        private void Update()
        {
            if (rectOriginal != null && rectOriginal != null)
            {
                rectToUpdate.sizeDelta = new Vector2(rectToUpdate.sizeDelta.x, rectOriginal.sizeDelta.y + offsetHeight);
            }
        }
    }
}