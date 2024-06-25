using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DraggableObject : MonoBehaviour
    {
        [SerializeField] private float startDragDelay = 1f;
        [SerializeField] private Vector2 maxDragDistance = new Vector2(5f, 5f); // max distance to drag in x and y separately

        private Vector3 offset;
        private bool isDragging;
        private float pressTime;

        private void Update()
        {
            if (isDragging)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 newTargetPos = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);

                newTargetPos.x = Mathf.Clamp(newTargetPos.x, -maxDragDistance.x, maxDragDistance.x);
                newTargetPos.y = Mathf.Clamp(newTargetPos.y, -maxDragDistance.y, maxDragDistance.y);

                transform.position = newTargetPos;
            }
            else if (Input.GetMouseButton(0) && Time.time - pressTime >= startDragDelay)
            {
                pressTime = float.MaxValue;
                isDragging = true;
            }
        }

        private void OnMouseDown()
        {
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
        
    }
}