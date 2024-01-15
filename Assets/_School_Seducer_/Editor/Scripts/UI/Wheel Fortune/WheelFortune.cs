using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Extensions;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using _School_Seducer_.Editor.Scripts.Utility;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    [SelectionBase]
    public class WheelFortune : MonoBehaviour
    {
        [Inject] private Bank _bank;
        [Inject] private EventManager _eventManager;

        [Header("Data")] 
        [SerializeField] private Previewer previewer;
        [SerializeField] private WheelFortuneData data;
        [SerializeField] private WheelSlot characterSlotPrefab;
        [SerializeField] private List<WheelSlot> slots;

        [Header("PushUps")] 
        [SerializeField] private Push giftSpinPush;
        [SerializeField] private RectTransform unlockNewStoryPush;

        [Header("UI elements")] 
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI textSpin;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Slider expSlider;
        [SerializeField] private Button spinButton;
        [SerializeField] private RectTransform scrollCharactersContent;
        [SerializeField] private RectTransform marker;
        [SerializeField] private RectTransform goToStopCharactersWheel;
        [SerializeField] private RectTransform goToStopWheel;

        private List<WheelSlot> _characterSlots = new();
        
        private RectTransform _spinButtonRect;
        private BoxCollider2D _winSlotCol;
        private BoxCollider2D _winSlotColCharacter;
        private WheelSlot _currentWinSlot;
        private Vector3 _goToStopWheelPosition;

        private readonly Color _activeButtonColor = Color.green;
        private readonly Color _inactiveButtonColor = Color.red;
        
        private float _rotationSpeed;
        private float _deceleration;

        private int _winGiftExp;
        
        private bool _isCharacters;
        private bool _isSpinning;
        private bool _winSlotCollidedStopCol;
        private bool _needStopWheel;
        private bool _needAnimate;

        private void Awake()
        {
            _spinButtonRect = spinButton.GetComponent<RectTransform>();
            _goToStopWheelPosition = goToStopWheel.transform.position;
            spinButton.AddListener(OnSpinButtonClicked);

            _eventManager.ChangeValueExperienceEvent += OnChangeValueExperience;

            _eventManager.UpdateTextMoneyEvent += UpdateGoldText;
            
            _eventManager.UpdateTextMoney();
        }

        private void UpdateGoldText()
        {
            goldText.text = _bank.Money.ToString();
        }

        private void Start()
        {
            gameObject.Deactivate();
            InitializeSlots();

            InitParametersData();

            previewer.Characters.ForEach(character => CheckResetCharacter(character.Data));
        }

        private void OnDestroy()
        {
            spinButton.RemoveListener(SetSpinWheelSlots);
            spinButton.RemoveListener(OnSpinButtonClicked);
            
            _eventManager.ChangeValueExperienceEvent -= OnChangeValueExperience;
            
            _eventManager.UpdateTextMoneyEvent -= UpdateGoldText;
        }
        
        private void OnChangeValueExperience(int newValue)
        {
            expSlider.value = newValue;
            Debug.Log($"Slider value changed to: {newValue}");
        }

        private void InitParametersData()
        {
            _rotationSpeed = data.rotationSpeed;
            
            _deceleration = Random.Range(data.decelerationMin, data.decelerationMax);
        }
        
        private void FixedUpdate()
        {
            if (_isSpinning && !_isCharacters)
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
                        _isCharacters = true;
                        CalculateResult();
                    }
                }
            }
            
            if (_isSpinning && _isCharacters)
            {
                float step = _rotationSpeed * Time.fixedDeltaTime;
                scrollCharactersContent.Translate(Vector2.down * step * Time.deltaTime);

                _winSlotCollidedStopCol = _winSlotColCharacter.OverlapPoint(goToStopCharactersWheel.transform.position);

                //Debug.Log("collided winSlot: " + _winSlotCollidedStopCol);  
                
                if (_needStopWheel)
                {
                    if (_rotationSpeed > 0)
                    {
                        _rotationSpeed -= data.decelerationCharactersWheel * Time.fixedDeltaTime;
                    }
                    else
                    {
                        _isSpinning = false;
                        _isCharacters = false;
                        CalculateResult();
                    }
                }
            }
        }

        private void OnSpinButtonClicked()
        {
            if (data.CanSpin(_bank.Data.Money) == false)
            {
                Debug.LogWarning("Not enough money to spin!");
                return;
            }
            
            if (scrollCharactersContent.childCount > 0)
                _bank.ChangeValueMoney(-data.moneyForSpin);

            ResetSpinStatus();
            StartCoroutine(HandleSpinButtonStatus());
        }

        // not need states for simple status
        private IEnumerator HandleSpinButtonStatus()
        {
            if (scrollCharactersContent.childCount <= 0)
            {
                SetSpinButtonUnavailableStatus();
                yield break;
            }
            
            ResetSpinStatus();
            
            if (_isCharacters == false) SetSpinWheelSlots();
            else
            {
                SetSpinWheelCharacters();
            }

            if (_needStopWheel && _isCharacters) _needStopWheel = false;
            
            SetSpinButtonNoneStatus();

            if (_isCharacters == false) yield return new WaitForSeconds(data.timeShowStopButton);

            SetSpinButtonStopStatus();
            
            SetSpinButtonNoneStatus();
            yield return new WaitUntil(StopWheel);

            yield return new WaitUntil(CalculateResult);

            CharacterData winCharacterData = null;
            
            if (_isCharacters)
            {
                characterName.text = _currentWinSlot.Data.name.ToUpper();
                
                giftSpinPush.SetDataParent(new DataParentText(characterName));
                
                yield return new WaitForSeconds(1.5f);
                
                Debug.Log($"Current win slot name is <color=red>{_currentWinSlot.Data.name}</color>");

                Character winCharacter = FindCharacterByWinSlot();

                if (winCharacter == null) yield return new WaitForSeconds(1);

                    winCharacterData = winCharacter.Data;

                Debug.Log("Found character after spin: " + winCharacter.Data.name);

                previewer.SetLockedConversation(winCharacter);

                if (winCharacter.Data.ConversationsUnlocked() == false)
                    previewer.StoryResolver.SetRolledConversation(winCharacter.Data.LockedConversation);
                
                winCharacter.Data.ChangeExperience(_winGiftExp);
            
                previewer.StoryResolver.SetSliderValue(winCharacter.Data.experience);
            
                _eventManager.UpdateTextExperience();
            
                previewer.StoryResolver.UpdateStatusViews();
            
                _eventManager.UpdateTextExperience();

                if (winCharacter.Data.allConversations[^1].isUnlocked == false)
                {
                    giftSpinPush.MakeTransitions();
                    
                    if (previewer.StoryResolver.StoryUnlocked)
                    {
                        ShowUnlockStoryPush();

                        if (winCharacter.Data.LockedConversation != winCharacter.Data.allConversations[^1])
                        {
                            winCharacter.Data.LockedConversation.isUnlocked = true;
                            winCharacter.Data.LockedConversation.isCompleted = false;
                            winCharacter.Data.experience = 0;
                        }

                        if (winCharacter.Data.allConversations[^1] == winCharacter.Data.LockedConversation)
                        {
                            winCharacter.Data.allConversations[^1].isUnlocked = true;
                            winCharacter.Data.allConversations[^1].isCompleted = false;
                        }
                        
                    }
                    else ShowGiftPush();
                
                    Debug.Log("Last conversation unlocked? = " + winCharacter.Data.LastConversationAvailable());
                }
            }
            else
                _winGiftExp = _currentWinSlot.Data.GetCostExp();

            yield return new WaitForSeconds(2);

            SetSpinButtonSpinStatus();

            if (_isCharacters) spinButton.onClick.Invoke();

            if (winCharacterData != null)
            {
                CheckResetCharacter(winCharacterData);
            }
        }

        private void ShowGiftPush()
        {
            StartCoroutine(giftSpinPush.transform.DoLocalScaleAndUnscale(this, Vector3.one, 2));
        }

        private void ShowUnlockStoryPush()
        {
            StartCoroutine(unlockNewStoryPush.DoLocalScaleAndUnscale(this, Vector3.one, 3));
        }

        private void SetSpinWheelCharacters()
        {
            _isSpinning = true;
            _isCharacters = true;
            _needStopWheel = false;

            _currentWinSlot = _isCharacters == false ? FindSlotForProbability(slots) : FindSlotForProbability(_characterSlots);

            if (_currentWinSlot == null)
            {
                return;
            }
            
            Debug.Log("Character slot is null? " + _currentWinSlot.name);
            
            //if (_currentWinSlot != null) giftSpinPush.SetDataParent(new DataParentImage(_currentWinSlot.Image, nameof(_currentWinSlot)));
            
            _winSlotCol = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();
            _winSlotColCharacter = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();

            //lockedConversationIsFound = _isCharacters && previewer.FindLockedConversation(_currentWinSlot.GetCurrentIcon());

            _eventManager.UpdateTextExperience();

            Debug.Log("winSlot: " + _currentWinSlot.Data.name);
        }

        private void SetSpinWheelSlots()
        {
            scrollCharactersContent.localPosition = new Vector2(0, 650);
            
            if (!_isSpinning)
            {
                _isSpinning = true;

                _currentWinSlot = _isCharacters == false ? FindSlotForProbability(slots) : FindSlotForProbability(_characterSlots);
                _winSlotCol = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                _winSlotColCharacter = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                
                giftSpinPush.SetDataParent(new DataParentImage(_currentWinSlot.Image, nameof(_currentWinSlot)));
                
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
            
            //if (_isCharacters == false) _isCharacters = true;

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

        private WheelSlot FindSlotForProbability(List<WheelSlot> wheelSlots)
        {
            if (wheelSlots.Count == 0)
            {
                return null;
            }
            
            float randomValue = Random.Range(1, 101);
            
            List<WheelSlot> eligibleSlots = new List<WheelSlot>();

            for (var index = wheelSlots.Count - 1; index >= 0; index--)
            {
                var slot = wheelSlots[index];
                Vector2 probabilityRange = slot.Data.GetProbabilityRange();

                if (randomValue >= probabilityRange.x && randomValue <= probabilityRange.y)
                {
                    eligibleSlots.Add(slot);
                }
            }

            Debug.Log("Count eligibleSlots: " + eligibleSlots.Count);

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
        
        private void SetSpinButtonUnavailableStatus()
        {
            SetSpinButtonColorInactive();
            textSpin.text = "MAX";
            spinButton.interactable = false;
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

        private Character FindCharacterByWinSlot()
        {
            foreach (var characterData in data.characters)
            {
                if (characterData.name != _currentWinSlot.Data.name) continue;
                
                Debug.Log("Found character data by win slot = <color=red>" + characterData.name + "</color>");

                WheelSlotData foundData = characterData;

                if (foundData == null) return null;

                foreach (var character in previewer.Characters)
                {
                    if (character.Data.name == foundData.name) return character;
                }
            }

            Debug.LogError("Can't find character by win slot");
            return null;
        }

        private void CheckResetCharacter(CharacterData characterData)
        {
            foreach (var conversation in characterData.allConversations)
            {
                if (conversation.isUnlocked == false) return;
            }

            for (int i = 0; i < scrollCharactersContent.childCount; i++)
            {
                 WheelSlot slotCharacterToReset = scrollCharactersContent.GetChild(i).GetComponent<WheelSlot>();

                if (slotCharacterToReset.Data.name == characterData.name)
                {
                    _characterSlots.Remove(slotCharacterToReset);
                    slotCharacterToReset.gameObject.Destroy();
                }
            }
        }

        private void InitializeSlots()
        {
            int charactersCount = data.characters.Count;
            int targetNumberOfSlots = charactersCount * 2;

            for (int i = 0; i < targetNumberOfSlots; i++)
            {
                var character = data.characters[i % charactersCount];
                WheelSlot characterSlot = Instantiate(characterSlotPrefab, scrollCharactersContent);
                characterSlot.Initialize(character, data.iconMoney);
                
                if (i < data.characters.Count)
                {
                    _characterSlots.Add(characterSlot);
                }
            }

            scrollCharactersContent.localPosition = new Vector2(0, 650);
            
            foreach (var slot in slots)
            {
                slot.Initialize(slot.Data, data.iconMoney);
            }
        }
    }
}