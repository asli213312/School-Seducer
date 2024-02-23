using System.Collections;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Chat.Refactor;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using _School_Seducer_.Editor.Scripts.Utility;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
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
        [Inject] private IChatStoryResolverModule _chatStoryResolver;

        [Header("Data")] 
        [SerializeField] private Previewer previewer;
        [SerializeField] private WheelFortuneData data;
        [SerializeField] private WheelSlot characterSlotPrefab;
        [SerializeField] private List<WheelSlot> slots;

        [Header("PushUps")] 
        [SerializeField] private Push giftSpinPush;
        [SerializeField] private Push unlockNewStoryPush;

        [Header("UI elements")]
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI textSpin;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Slider expSlider;
        [SerializeField] private Button extraSpinButton;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private RectTransform scrollCharactersContent;
        [SerializeField] private RectTransform marker;
        [SerializeField] private RectTransform goToStopCharactersWheel;
        [SerializeField] private RectTransform goToStopWheel;
        
        private List<WheelSlot> _characterSlots = new();
        private List<WheelSlot> _extraSlots = new();

        private TextMeshProUGUI _costSpinText;
        private RectTransform _spinButtonRect;
        private BoxCollider2D _winSlotCol;
        private BoxCollider2D _winSlotColCharacter;
        private WheelSlot _currentWinSlot;
        private Vector3 _goToStopWheelPosition;

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
            extraSpinButton.AddListener(OnExtraSpinButtonClicked);

            _eventManager.ChangeValueExperienceEvent += OnChangeValueExperience;

            _eventManager.UpdateTextMoneyEvent += UpdateGoldText;
            
            // open chat = buttons[0]
            unlockNewStoryPush.Buttons[0].AddListener(() =>
            {
                unlockNewStoryPush.gameObject.Deactivate();
                gameObject.Deactivate(1);
                previewer.OnCharacterSelected(previewer.CurrentCharacter);
            }); 
        }

        private void UpdateGoldText()
        {
            goldText.text = _bank.Money.ToString();
        }

        private void Start()
        {
            InitializeSlots();
            InitParametersData();

            CheckResetCharactersOutWheel();

            _costSpinText = spinButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            _costSpinText.text = data.moneyForSpin + " COIN";
            
            _eventManager.UpdateTextMoney();

            void CheckResetCharactersOutWheel()
            {
                previewer.Characters.ForEach(character => CheckResetCharacter(character.Data));
            }
        }

        private void OnDestroy()
        {
            //spinButton.RemoveListener(SetSpinWheelSlots);
            spinButton.RemoveListener(OnSpinButtonClicked);
            extraSpinButton.RemoveListener(OnExtraSpinButtonClicked);
            
            _eventManager.ChangeValueExperienceEvent -= OnChangeValueExperience;
            
            _eventManager.UpdateTextMoneyEvent -= UpdateGoldText;
            
            // open chat = buttons[0]
            unlockNewStoryPush.Buttons[0].RemoveListener(() =>
            {
                unlockNewStoryPush.gameObject.Deactivate();
                gameObject.Deactivate(1);
                previewer.OnCharacterSelected(previewer.CurrentCharacter);
            }); 
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

        private void OnExtraSpinButtonClicked()
        {
            if (data.CanSpin(_bank.Data.Money, 10f) == false)
            {
                Debug.LogWarning("Not enough money to extra spin!");
                return;
            }
            
            if (scrollCharactersContent.childCount > 0)
                _bank.ChangeValueMoney(-data.moneyForSpin * 10);

            ResetSpinStatus();
            StartCoroutine(HandleExtraSpinButtonStatus());
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
        
        private IEnumerator HandleExtraSpinButtonStatus()
        {
            if (scrollCharactersContent.childCount <= 0)
            {
                SetSpinButtonUnavailableStatus();
                yield break;
            }
            
            ResetSpinStatus();
            
            if (_isCharacters == false) SetSpinWheelSlots(true);
            else
            {
                SetSpinWheelCharacters(true);
            }

            if (_needStopWheel && _isCharacters) _needStopWheel = false;
            
            SetSpinButtonNoneStatus();

            SetSpinButtonNoneStatus();

            if (_isCharacters == false)
            {
                _rotationSpeed -= data.rotationSlowSpeed * Time.fixedDeltaTime;
                yield return new WaitForSeconds(data.secondsSlowSpdSlots);
            }

            yield return new WaitUntil(StopWheel);
            
            yield return new WaitUntil(() => _rotationSpeed <= _deceleration);

            yield return new WaitUntil(CalculateResult);

            CharacterData winCharacterData = null;
            List<CharacterData> winCharactersData = new();
            
            if (_isCharacters)
            {
                characterName.text = _extraSlots[^1].Data.name.ToUpper();
                
                Debug.Log($"Last win slot name by EXTRA spin is <color=red>{_extraSlots[^1].Data.name}</color>");
                
                giftSpinPush.SetDataParent(new DataParentText(characterName));
                
                yield return new WaitForSeconds(1.5f);

                List<Character> winCharacters = new();

                for (int i = 0; i < 10; i++)
                {
                    Character winCharacter = FindCharacterByWinSlot();
                    winCharactersData.Add(winCharacter.Data);
                    winCharacters.Add(winCharacter);
                    
                    Debug.Log($"Found {i + 1} character after EXTRA spin: " + winCharacter.Data.name);
                    
                    previewer.SetLockedConversation(winCharacter);
                }

                previewer.CurrentCharacter = winCharacters[^1];

                ChatStoryResolver chatStoryResolver = _chatStoryResolver as ChatStoryResolver;

                foreach (var winCharacter in winCharactersData)
                {
                    if (winCharacter.ConversationsUnlocked() == false)
                    {
                        previewer.StoryResolver.SetRolledConversation(winCharacter.LockedConversation);
                        chatStoryResolver.SetRolledConversation(winCharacter.LockedConversation);
                    }
                    
                    winCharacter.ChangeExperience(_winGiftExp);
                    OnChangeValueExperience(winCharacter.experience);

                    previewer.StoryResolver.SetSliderValue(winCharacter.experience);
                    chatStoryResolver.SetSliderValue(winCharacter.experience);
                }

                previewer.StoryResolver.UpdateStatusViews();
                _chatStoryResolver.UpdateView();

                _eventManager.UpdateTextExperience();
            
                previewer.StoryResolver.UpdateStatusViews();
                _chatStoryResolver.UpdateView();
            
                _eventManager.UpdateTextExperience();

                foreach (var characterData in winCharactersData)
                {
                    if (characterData.allConversations[^1].isUnlocked == false)
                    {
                        giftSpinPush.MakeTransitions();
                    
                        if (previewer.StoryResolver.StoryUnlocked || _chatStoryResolver is ChatStoryResolver
                            {
                                StoryUnlocked: true
                            }
                           )
                        {
                            ShowUnlockStoryPush();

                            if (characterData.LockedConversation != characterData.allConversations[^1])
                            {
                                characterData.LockedConversation.isUnlocked = true;
                                characterData.LockedConversation.isCompleted = false;
                                characterData.experience = 0;
                            }

                            if (characterData.allConversations[^1] == characterData.LockedConversation)
                            {
                                characterData.allConversations[^1].isUnlocked = true;
                                characterData.allConversations[^1].isCompleted = false;
                            }
                        
                        }
                        else ShowGiftPush();
                
                        Debug.Log("Last conversation unlocked? = " + characterData.LastConversationAvailable());
                    }
                }
            }
            else
                _winGiftExp = _currentWinSlot.Data.GetCostExp();

            yield return new WaitForSeconds(2);

            SetSpinButtonSpinStatus();

            if (_isCharacters) spinButton.onClick.Invoke();

            winCharactersData.ForEach(CheckResetCharacter);
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

            SetSpinButtonNoneStatus();

            if (_isCharacters == false)
            {
                _rotationSpeed -= data.rotationSlowSpeed * Time.fixedDeltaTime;
                yield return new WaitForSeconds(data.secondsSlowSpdSlots);
            }

            yield return new WaitUntil(StopWheel);
            
            yield return new WaitUntil(() => _rotationSpeed <= _deceleration);

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

                previewer.CurrentCharacter = winCharacter;

                Debug.Log("Found character after spin: " + winCharacter.Data.name);

                previewer.SetLockedConversation(winCharacter);
                
                ChatStoryResolver chatStoryResolver = _chatStoryResolver as ChatStoryResolver;

                if (winCharacter.Data.ConversationsUnlocked() == false)
                {
                    previewer.StoryResolver.SetRolledConversation(winCharacter.Data.LockedConversation);
                    chatStoryResolver.SetRolledConversation(winCharacter.Data.LockedConversation);
                }

                previewer.StoryResolver.UpdateStatusViews();
                _chatStoryResolver.UpdateView();
                
                winCharacter.Data.ChangeExperience(_winGiftExp);
                OnChangeValueExperience(winCharacter.Data.experience);

                previewer.StoryResolver.SetSliderValue(winCharacter.Data.experience);
                chatStoryResolver.SetSliderValue(winCharacter.Data.experience);
            
                _eventManager.UpdateTextExperience();
            
                previewer.StoryResolver.UpdateStatusViews();
                _chatStoryResolver.UpdateView();
            
                _eventManager.UpdateTextExperience();

                if (winCharacter.Data.allConversations[^1].isUnlocked == false)
                {
                    giftSpinPush.MakeTransitions();
                    
                    if (previewer.StoryResolver.StoryUnlocked || _chatStoryResolver is ChatStoryResolver
                        {
                            StoryUnlocked: true
                        }
                       )
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
            StartCoroutine(giftSpinPush.transform.DoLocalScaleAndUnscale(this, new Vector3(0.9f, 0.9f, 0.9f)));
        }

        private void ShowUnlockStoryPush()
        {
            StartCoroutine(unlockNewStoryPush.transform.DoLocalScaleAndUnscale(this, new Vector3(0.9f, 0.9f, 0.9f)));
        }

        private void SetSpinWheelCharacters(bool isExtraSpin = false)
        {
            _isSpinning = true;
            _isCharacters = true;
            _needStopWheel = false;

            if (isExtraSpin)
            {
                for (int i = 0; i < 10; i++)
                {
                    WheelSlot winSlot = _isCharacters == false ? FindSlotForProbability(slots) : FindSlotForProbability(_characterSlots); 
                    _extraSlots.Add(winSlot);
                }
            }

            _currentWinSlot = _isCharacters == false ? FindSlotForProbability(slots) : FindSlotForProbability(_characterSlots);

            if (_currentWinSlot == null)
            {
                return;
            }
            
            _deceleration = data.decelerationCharactersWheel;
            _rotationSpeed = data.rotationSpeedCharacters;
            
            Debug.Log("Character slot is null? " + _currentWinSlot.name);
            
            if (_currentWinSlot != null) giftSpinPush.InitializeTransitionParent(0, new DataParentImage(_currentWinSlot.Image, null));
            
            _winSlotCol = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();
            _winSlotColCharacter = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();

            //lockedConversationIsFound = _isCharacters && previewer.FindLockedConversation(_currentWinSlot.GetCurrentIcon());

            _eventManager.UpdateTextExperience();

            Debug.Log("winSlot: " + _currentWinSlot.Data.name);
        }

        private void SetSpinWheelSlots(bool isExtraSpin = false)
        {
            scrollCharactersContent.localPosition = new Vector2(0, 2050);

            if (!_isSpinning)
            {
                _isSpinning = true;
                
                if (isExtraSpin)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        WheelSlot newSlot = _isCharacters == false ? FindSlotForProbability(slots) : FindSlotForProbability(_characterSlots);
                        _extraSlots.Add(newSlot);
                    }
                }

                _currentWinSlot = _isCharacters == false ? FindSlotForProbability(slots) : FindSlotForProbability(_characterSlots);
                _winSlotCol = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                _winSlotColCharacter = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();

                _rotationSpeed = data.rotationSpeed;

                giftSpinPush.SetDataParent(new DataParentImage(_currentWinSlot.Image, nameof(_currentWinSlot)));
                
                giftSpinPush.MakeTransitions();

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
            _deceleration = Random.Range(data.decelerationMin, data.decelerationMax);
            _isSpinning = false;
            _needStopWheel = false;
            
            if (_extraSlots.Count > 0) _extraSlots.Clear();
        }

        private void SetSpinButtonNoneStatus()
        {
            textSpin.text = "";
            _costSpinText.gameObject.Deactivate();
            spinButton.interactable = false;
            closeButton.interactable = false;
        }

        private void SetSpinButtonSpinStatus()
        {
            textSpin.text = "SPIN!";
            _costSpinText.gameObject.Activate();
            spinButton.interactable = true;
            closeButton.interactable = true;
        }

        private void SetSpinButtonUnavailableStatus()
        {
            textSpin.text = "MAX";
            spinButton.interactable = false;
            _costSpinText.gameObject.Activate();
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
            int targetNumberOfSlots = charactersCount * 4;

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

            scrollCharactersContent.localPosition = new Vector2(0, 2050);
            
            foreach (var slot in slots)
            {
                slot.Initialize(slot.Data, data.iconMoney);
            }
        }
    }
}