using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ObjWorldFollower : MonoBehaviour
    {
        [SerializeField] private RectTransform uiElement; 
        [SerializeField] private Transform objectToFollow; 
        [SerializeField] private Vector3 offset; 
        [SerializeField] private Camera uiCamera; 

        private Canvas canvas;
    
        private void Awake()
        {
            canvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, objectToFollow.position + offset);
            uiElement.anchoredPosition = screenPoint/canvas.scaleFactor - canvas.pixelRect.size/2;
        }
    }
}