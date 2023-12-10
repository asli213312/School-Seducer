using System.Collections;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    [SelectionBase]
    public class WheelFortune : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private WheelFortuneData data;
        [SerializeField] private WheelSlot[] slots;
        
        [Header("UI elements")]
        [SerializeField] private Text textSpin;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Button spinButton;
        [SerializeField] private RectTransform marker;
        [SerializeField] private RectTransform goToStopWheel;

        private RectTransform _spinButtonRect;
        private BoxCollider2D _winSlotCol;
        private WheelSlot _currentWinSlot;
        private Vector3 _goToStopWheelPosition;
        
        private readonly Color _activeButtonColor = Color.green;
        private readonly Color _inactiveButtonColor = Color.red;

        private bool _isSpinning;
        private float _targetRotation;
        private float _rotationSpeed;
        private float _deceleration;

        private bool _winSlotCollidedStopCol;
        private bool _needStopWheel;

        private void Awake()
        {
            _spinButtonRect = spinButton.GetComponent<RectTransform>();
            _goToStopWheelPosition = goToStopWheel.transform.position;
            spinButton.AddListener(OnSpinButtonClicked);
        }

        private void Start()
        {
            gameObject.Deactivate();
            InitializeSlots();

            InitParametersData();
        }

        private void OnDestroy()
        {
            spinButton.RemoveListener(SpinWheel);
            spinButton.RemoveListener(OnSpinButtonClicked);
        }

        private void InitParametersData()
        {
            _rotationSpeed = data.rotationSpeed;
            _deceleration = Random.Range(data.decelerationMin, data.decelerationMax);
        }
        
        private void FixedUpdate()
        {
            if (_isSpinning)
            {
                float step = _rotationSpeed * Time.fixedDeltaTime;
                wheelTransform.Rotate(Vector3.forward * step);

                _winSlotCollidedStopCol = _winSlotCol.OverlapPoint(_goToStopWheelPosition);

                //Debug.Log("collided winSlot: " + _winSlotCollidedStopCol);  
                
                if (_needStopWheel)
                {
                    if (_rotationSpeed > 0)
                    {
                        _rotationSpeed -= _deceleration * Time.fixedDeltaTime;
                    }
                    else
                    {
                        _isSpinning = false;
                        CalculateResult();
                    }
                }
            }
        }

        private void OnSpinButtonClicked()
        {
            ResetSpinStatus();
            StartCoroutine(HandleSpinButtonStatus());
        }

        // not need states for simple status
        private IEnumerator HandleSpinButtonStatus()
        {
            SpinWheel();
            SetSpinButtonNoneStatus();
            yield return new WaitForSeconds(data.timeShowStopButton);
            
            SetSpinButtonStopStatus();

            yield return new WaitUntil(SpinButtonClicked);
            SetSpinButtonNoneStatus();
            yield return new WaitUntil(StopWheel);

            yield return new WaitUntil(CalculateResult);
            yield return new WaitForSeconds(2);
            SetSpinButtonSpinStatus();
        }

        private void SpinWheel()
        {
            if (!_isSpinning)
            {
                _isSpinning = true;

                _currentWinSlot = FindSlotForProbability();
                _winSlotCol = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                Debug.Log("winSlot: " + _currentWinSlot.Data.name);
                
                //_targetRotation = 360 * Random.Range(3, 6) + (360 / slots.Length) * Random.Range(0, slots.Length);
            }
        }

        private bool StopWheel()
        {
            if (_winSlotCollidedStopCol && _needStopWheel == false)
            {
                Debug.Log("winSlot COLLIDED with stopCol");
                _needStopWheel = true;
                return true;
            }

            return false;
        }

        private bool CalculateResult()
        {
            RaycastHit2D hit = GetRaycastDownMarker();

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                Debug.Log("hitObject: " + hitObject.name);
                WheelSlot wheelSlot = hitObject.GetComponent<WheelSlot>();
                Debug.Log("win prize is: " + wheelSlot.gameObject.name);

                return true;
            }
            
            return false;
        }

        private RaycastHit2D GetRaycastDownMarker()
        {
            Vector2 rayOrigin = marker.position;
            Vector2 rayDirection = -marker.up;

            return Physics2D.Raycast(rayOrigin, rayDirection);
        }

        private WheelSlot FindSlotForProbability()
        {
            if (slots.Length == 0)
            {
                return null;
            }
            
            float randomValue = Random.Range(1, 101);
            
            List<WheelSlot> eligibleSlots = new List<WheelSlot>();

            foreach (var slot in slots)
            {
                Vector2 probabilityRange = slot.GetProbabilityRange();

                if (randomValue >= probabilityRange.x && randomValue <= probabilityRange.y)
                {
                    eligibleSlots.Add(slot);
                    Debug.Log(eligibleSlots.Count + ": " + slot.gameObject.name);
                }
            }
            
            WheelSlot selectedSlot = eligibleSlots[Random.Range(0, eligibleSlots.Count)];

            return selectedSlot;
        }

        private bool SpinButtonClicked()
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle
                (_spinButtonRect, Input.mousePosition, 
                    Camera.main, out var buttonPosition))
            {
                return Input.GetMouseButtonDown(0) && _spinButtonRect.rect.Contains(buttonPosition);
            }

            return false;
        }

        private void ResetSpinStatus()
        {
            _rotationSpeed = data.rotationSpeed;
            _isSpinning = false;
            _needStopWheel = false;
        }

        private void SetSpinButtonNoneStatus()
        {
            textSpin.text = "";
            spinButton.interactable = false;
        }

        private void SetSpinButtonSpinStatus()
        {
            SetSpinButtonActive();
            textSpin.text = "SPIN";
            spinButton.interactable = true;
        }

        private void SetSpinButtonStopStatus()
        {
            SetSpinButtonColorInactive();
            textSpin.text = "STOP";
            spinButton.interactable = true;
        }

        private void SetSpinButtonColorInactive()
        {
            SetSpinButtonColor(_inactiveButtonColor);
        }

        private void SetSpinButtonActive()
        {
            SetSpinButtonColor(_activeButtonColor);
        }

        private void SetSpinButtonColor(Color colorStatus)
        {
            spinButton.GetComponent<Image>().color = colorStatus;
        }

        private void InitializeSlots()
        {
            foreach (var slot in slots)
            {
                slot.Initialize(data.iconMoney);
            }
        }
    }
}