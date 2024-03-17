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
        [SerializeField] private RectTransform goToStopCharacter;
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
            // Получаем позицию объекта goToStopCharacter относительно content
            Vector3 goToStopCharacterLocalPosition = goToStopCharacter.localPosition;

            // Получаем позицию выпавшего элемента относительно content
            Vector3 fallenElementLocalPosition = _rectWinSlot.localPosition;

            // Рассчитываем, насколько нужно прокрутить content, чтобы выпавший элемент был на нужной позиции относительно goToStopCharacter
            float yOffset = -(fallenElementLocalPosition.y - goToStopCharacterLocalPosition.y);

            // Прокручиваем content
            float scrollSpeed = SpinHandler.Data.speedSpinCharacters; // Скорость прокрутки
            float duration = SpinHandler.Data.durationSpinCharacters; // Время прокрутки
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Рассчитываем новую позицию content
                Vector2 newAnchoredPosition = SpinHandler.scrollCharactersContent.anchoredPosition;
                newAnchoredPosition.y += (yOffset / duration) * Time.deltaTime * scrollSpeed;

                // Применяем новую позицию
                SpinHandler.scrollCharactersContent.anchoredPosition = newAnchoredPosition;

                // Увеличиваем прошедшее время
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Завершаем прокрутку точно к целевой позиции
            SpinHandler.scrollCharactersContent.anchoredPosition = 
                new Vector2(SpinHandler.scrollCharactersContent.anchoredPosition.x, 
                    -_rectWinSlot.localPosition.y + goToStopCharacter.localPosition.y);
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