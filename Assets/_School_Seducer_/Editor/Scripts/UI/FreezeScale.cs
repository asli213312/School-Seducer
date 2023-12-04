using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class FreezeScale : MonoBehaviour
    {
        private RectTransform rectTransform;
        [SerializeField] public Vector2 originalSizeDelta;
        [SerializeField] private Vector2 anchorMin;
        [SerializeField] private Vector2 anchorMax;
        public Vector2 AnchorMin { get => anchorMin; set => anchorMin = value; }

        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void LateUpdate()
        {
            // Восстанавливаем sizeDelta к оригинальному значению
            rectTransform.sizeDelta = originalSizeDelta;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}