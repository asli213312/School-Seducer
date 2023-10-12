using System;
using System.Collections;
using UnityEngine;

public class BallInHole : MonoBehaviour, IMiniGame
{
    [SerializeField] private CursorController cursor;
    [SerializeField] private GameObject availablePlace;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject hole;
    [Header("Options")]
    [SerializeField] private float maxGameTime = 10f;
    [SerializeField] private float holeRadius = 1f;
    [SerializeField] private bool useMouseDrag;
    
    //[ShowIf("UseCursorTouch")]
    [SerializeField] private float powerPush = 0.5f;
    private bool UseCursorTouch() => !useMouseDrag;
    
    private float _startTime;
    public bool IsGameFinished { get; set; }
    public event Action<bool> OnMiniGameFinished;

    private Vector3 _fingerPosition;
    
    private bool _isDragging;
    private CapsuleCollider2D _ballCollider;
    private Rigidbody2D _rbBall;

    private void OnEnable()
    {
        StartCoroutine(StartGame());

        _ballCollider = ball.GetComponent<CapsuleCollider2D>();
        _rbBall = ball.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if (useMouseDrag)
            {
                HandleMouseDrag();
            }
            else
            {
                HandleCursorTouch();
            }
        }
        
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                _fingerPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 0));
                ball.transform.position = new Vector3(_fingerPosition.x, _fingerPosition.y, ball.transform.position.z);
            }
        }
    }
    
    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        
            if (_ballCollider.OverlapPoint(mousePosition))
            {
                _isDragging = true;
            }
        }
    
        if (_isDragging && Input.GetMouseButton(0))
        {
            _fingerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Vector3 clampedPosition = ClampPositionToAvailablePlace(_fingerPosition);
            ball.transform.position = clampedPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }
    }
    
    private void HandleCursorTouch()
    {
        _fingerPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Vector2 direction = ball.transform.position - _fingerPosition;
        float distance = direction.magnitude;
        direction.Normalize();

        Vector3 newPosition = _rbBall.position + direction * powerPush / distance * Time.fixedDeltaTime;
        Vector3 clampedPosition = ClampPositionToAvailablePlace(newPosition);
        _rbBall.MovePosition(clampedPosition);
    }

    private void MiniGameFinished(bool result)
    {
        if (OnMiniGameFinished != null)
        {
            OnMiniGameFinished(result);
        }
    }

    public IEnumerator StartGame()
    {
        float startTime = Time.time;
        bool miniGameResult = false;

        SetRandomBallAndHolePositions();

        while (Time.time - startTime < maxGameTime)
        {
            if (Vector3.Distance(ball.transform.position, hole.transform.position) < holeRadius)
            {
                miniGameResult = true;
                break;
            }
            
            yield return new WaitForSeconds(0.1f);
        }
        
        MiniGameFinished(miniGameResult);
        IsGameFinished = true;
    }

    private void SetRandomBallAndHolePositions()
    {
        Vector2 positionAvailablePlace = availablePlace.transform.localPosition;
        float positionX = positionAvailablePlace.x = 0.45f;
        float positionY = positionAvailablePlace.y = 0.4f;
        
        float randomPositionX = UnityEngine.Random.Range(-positionX, positionX);
        
        float randomPositionHoleY = UnityEngine.Random.Range(0.3f, 0.5f); 
        float randomPositionBallY = UnityEngine.Random.Range(-positionY, positionY);

        float holePositionRandomY = UnityEngine.Random.Range(-randomPositionHoleY, randomPositionHoleY);

        ball.transform.localPosition = new Vector3(-randomPositionX * 1f, randomPositionBallY);
        hole.transform.localPosition = new Vector3(randomPositionX * 0.5f, holePositionRandomY);
    }
    
    private Vector3 ClampPositionToAvailablePlace(Vector3 position)
    {
        float availablePlacePositionX = availablePlace.transform.position.x;
        float availablePlacePositionY = availablePlace.transform.position.y;
        float availablePlaceScaleX = availablePlace.transform.localScale.x;
        float availablePlaceScaleY = availablePlace.transform.localScale.y;

        const float offsetMultiplierX = 1.7f;
        const float offsetMultiplierY = 2.9f;

        float clampedMinX = availablePlacePositionX - availablePlaceScaleX / offsetMultiplierX;
        float clampedMaxX = availablePlacePositionX + availablePlaceScaleX / offsetMultiplierX;
        float clampedMinY = availablePlacePositionY - availablePlaceScaleY / offsetMultiplierY;
        float clampedMaxY = availablePlacePositionY + availablePlaceScaleY / offsetMultiplierY;

        float clampedX = Mathf.Clamp(position.x, clampedMinX, clampedMaxX);
        float clampedY = Mathf.Clamp(position.y, clampedMinY, clampedMaxY);

        return new Vector3(clampedX, clampedY, 0);
    }

    private Vector2 CalculateRandomPosition(float maxX, float maxY)
    {
        float randomX = UnityEngine.Random.Range(-maxX, maxX);
        float randomY = UnityEngine.Random.Range(-maxY, maxY);

        return new Vector2(randomX, randomY);
    }
}