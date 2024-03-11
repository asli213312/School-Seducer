using System;
using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class SpinHandlerLerp : SpinHandlerBase
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Button spinButton;

        [Header("Options")] 
        [SerializeField] private float spinSpeed;
        [SerializeField] private float duration;

        private RectTransform _content;
        private RectTransform _rectWinSlot;
        private WheelSlot _currentWinSlot;

        private float _targetY;
        private float _spinSpeed;
        
        private bool _isSpinning;

        private void Awake()
        {
            spinButton.AddListener(OnSpinButtonClicked);
        }

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            spinButton.RemoveListener(OnSpinButtonClicked);
        }

        private void Initialize()
        {
            _content = SpinHandler.scrollCharactersContent;
            InitParametersData();
        }

        private void InitParametersData()
        {
            _spinSpeed = spinSpeed;
        }

        private void OnSpinButtonClicked()
        {
            TryBuySpin();
            ResetStatusSpin();
            
            InstallContent();
            
            ScrollToTarget();
        }

        private void ResetStatusSpin()
        {
            _isSpinning = false;
        }

        private void ScrollToTarget()
        {
            _targetY = _rectWinSlot.anchoredPosition.y;
            _isSpinning = true;
            StartCoroutine(ScrollCoroutine());
        }

        private IEnumerator ScrollCoroutine()
        {
            float elapsedTime = 0f;

            while (_isSpinning)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                
                float newY = Mathf.Lerp(_content.anchoredPosition.y, _targetY, t * t);
                _content.anchoredPosition = new Vector2(_content.anchoredPosition.x, newY);

                if (Mathf.Approximately(_content.anchoredPosition.y, _targetY))
                {
                    _isSpinning = false;
                }

                yield return null;
            }
        }

        private void InstallContent()
        {
            EnableSpinning();
            SpinHandler.scrollCharactersContent.localPosition = new Vector2(0, int.MaxValue);

            _currentWinSlot = SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);
            
            if (_currentWinSlot == null)
            {
                return;
            }

            Debug.Log("Character slot is null? " + _currentWinSlot.name);

            if (_currentWinSlot != null)
            {
                SpinHandler.giftSpinPush.InitializeTransitionParent(0, new DataParentImage(_currentWinSlot.Image, null));
                Pushes.giftPush.GetComponent<Push>().InitializeTransitionParent(0, new DataParentImage(_currentWinSlot.Image, null));
            }

            _rectWinSlot = _currentWinSlot.GetComponent<RectTransform>();
            //_winSlotCol = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();
            //_winSlotColCharacter = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();

            EventManager.UpdateTextExperience();

            Debug.Log("winSlot: " + _currentWinSlot.Data.name, _currentWinSlot.gameObject);
        }
        
        private void EnableSpinning() => _isSpinning = true;

        protected override void Spin()
        {
            
        }

        protected override void Stop()
        {
            
        }
    }
}