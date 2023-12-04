using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CursorController : MonoBehaviour
{
    [SerializeField] private float cursorRadius = 0.3f;
    
    private CircleCollider2D _cursorCollider;

    private void Awake()
    {
        _cursorCollider = GetComponent<CircleCollider2D>();
        _cursorCollider.radius = cursorRadius;
    }

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        transform.position = mousePosition;
    }

    public bool IsTouching(CapsuleCollider2D ballCollider)
    {
        return _cursorCollider.IsTouching(ballCollider);
    }
}
