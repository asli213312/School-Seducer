using UnityEngine;
using UnityEngine.EventSystems;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class ReturnableDraggableObject : MonoBehaviour
    {
        [SerializeField] private float returnSpeed = 10f;
        [SerializeField] private float maxDragDistance = 5f;
        [SerializeField] private float startDragDelay = 1f;
        [SerializeField] private GameObject[] unclickableElements;

        private Vector3 startPosition;
        private Vector3 offset;
        private bool isDragging;
        private float pressTime;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            if (isDragging)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 targetPosition = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);

                targetPosition = Vector3.ClampMagnitude(targetPosition - startPosition, maxDragDistance) + startPosition;

                transform.position = targetPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * returnSpeed);
            }
        }

        private void OnMouseDown()
        {
            if (IsAnyElementOpen() || IsPointerOverUIElement())
            {
                return;
            }

            pressTime = Time.time;
            isDragging = false;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = transform.position - mousePosition;
        }

        private void OnMouseUp()
        {
            pressTime = float.MaxValue;
            isDragging = false;
        }

        private void OnMouseDrag()
        {
            if (!isDragging && Time.time - pressTime >= startDragDelay)
            {
                isDragging = true;
            }
        }

        private bool IsAnyElementOpen()
        {
            foreach (var element in unclickableElements)
            {
                if (element.activeSelf)
                {
                    return true;
                }
            }
            return false;
        }
        
        private bool IsPointerOverUIElement()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}