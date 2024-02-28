using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat.Refactor;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class SpinHandler : SpinHandlerBase
    {
        [Header("UI elements")]
        [SerializeField] private TextMeshProUGUI textSpin;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button spinButton;
        [SerializeField] private RectTransform marker;
        [SerializeField] private RectTransform goToStopCharactersWheel;
        [SerializeField] private RectTransform goToStopWheel;

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
        }

        private void OnDestroy()
        {
            spinButton.RemoveListener(OnSpinButtonClicked);
        }

        private void Start()
        {
            InitParametersData();
            
            _costSpinText = spinButton.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            _costSpinText.text = SpinHandler.Data.moneyForSpin + " COIN";
        }
        
        private void InitParametersData()
        {
            _rotationSpeed = SpinHandler.Data.rotationSpeed;
            
            _deceleration = Random.Range(SpinHandler.Data.decelerationMin, SpinHandler.Data.decelerationMax);
        }

        protected override void Spin()
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
                SpinHandler.scrollCharactersContent.Translate(Vector2.down * step * Time.deltaTime);

                _winSlotCollidedStopCol = _winSlotColCharacter.OverlapPoint(goToStopCharactersWheel.transform.position);

                if (_needStopWheel)
                {
                    if (_rotationSpeed > 0)
                    {
                        _rotationSpeed -= SpinHandler.Data.decelerationCharactersWheel * Time.fixedDeltaTime;
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

        protected override void Stop()
        {
            
        }
        
        private bool NeedStopWheel()
        {
            if (_winSlotCollidedStopCol && _needStopWheel == false)
            {
                Debug.Log("winSlot COLLIDED with stopCol");
                _needStopWheel = true;
                return true;
            }

            return false;
        }

        private void OnSpinButtonClicked()
        {
            if (SpinHandler.Data.CanSpin(Bank.Data.Money) == false)
            {
                Debug.LogWarning("Not enough money to spin!");
                return;
            }
            
            if (SpinHandler.scrollCharactersContent.childCount > 0)
                Bank.ChangeValueMoney(-SpinHandler.Data.moneyForSpin);

            ResetSpinStatus();
            StartCoroutine(HandleSpinButtonStatus());
        }

        private IEnumerator HandleSpinButtonStatus()
        {
            if (SpinHandler.scrollCharactersContent.childCount <= 0)
            {
                SetSpinButtonUnavailableStatus();
                yield break;
            }
            
            ResetSpinStatus();
            
            if (_isCharacters == false) SetSpinWheelSlots();
            else
            {
                SetSpinWheelCharacters();
                SpinHandler.charactersLockedSlot.gameObject.Deactivate();
            }

            if (_needStopWheel && _isCharacters) _needStopWheel = false;
            
            SetSpinButtonNoneStatus();

            SetSpinButtonNoneStatus();

            if (_isCharacters == false)
            {
                _rotationSpeed -= SpinHandler.Data.rotationSlowSpeed * Time.fixedDeltaTime;
                yield return new WaitForSeconds(SpinHandler.Data.secondsSlowSpdSlots);
            }

            yield return new WaitUntil(NeedStopWheel);
            
            yield return new WaitUntil(() => _rotationSpeed <= _deceleration);

            yield return new WaitUntil(CalculateResult);

            CharacterData winCharacterData = null;
            
            if (_isCharacters)
            {
                characterName.text = _currentWinSlot.Data.name.ToUpper();
                characterName.gameObject.Activate(0.5f);
                
                SpinHandler.giftSpinPush.SetDataParent(new DataParentText(characterName));
                
                yield return new WaitForSeconds(1.5f);
                
                Debug.Log($"Current win slot name is <color=red>{_currentWinSlot.Data.name}</color>");

                Character winCharacter =  FindCharacterByWinSlot();

                if (winCharacter == null) yield return new WaitForSeconds(1);

                winCharacterData = winCharacter.Data;

                SpinHandler.Previewer.CurrentCharacter = winCharacter;

                Debug.Log("Found character after spin: " + winCharacter.Data.name);

                SpinHandler.Previewer.SetLockedConversation(winCharacter);
                
                ChatStoryResolver chatStoryResolver = ChatStoryResolver as ChatStoryResolver;

                if (winCharacter.Data.ConversationsUnlocked() == false)
                {
                    SpinHandler.Previewer.StoryResolver.SetRolledConversation(winCharacter.Data.LockedConversation);
                    chatStoryResolver.SetRolledConversation(winCharacter.Data.LockedConversation);
                }

                SpinHandler.Previewer.StoryResolver.UpdateStatusViews();
                ChatStoryResolver.UpdateView();
                
                winCharacter.Data.ChangeExperience(_winGiftExp);
                SpinHandler.OnChangeValueExperience(winCharacter.Data.experience);

                SpinHandler.Previewer.StoryResolver.SetSliderValue(winCharacter.Data.experience);
                chatStoryResolver.SetSliderValue(winCharacter.Data.experience);
            
                EventManager.UpdateTextExperience();
            
                SpinHandler.Previewer.StoryResolver.UpdateStatusViews();
                ChatStoryResolver.UpdateView();
            
                EventManager.UpdateTextExperience();

                if (winCharacter.Data.allConversations[^1].isUnlocked == false)
                {
                    if (SpinHandler.Previewer.StoryResolver.StoryUnlocked || ChatStoryResolver is ChatStoryResolver
                        {
                            StoryUnlocked: true
                        }
                       )
                    {
                        SpinHandler.ShowUnlockStoryPush();

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
                    else SpinHandler.ShowGiftPush();
                
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
                SpinHandler.CheckResetCharacter(winCharacterData);
            }
        }
        
        private void SetSpinWheelCharacters()
        {
            _isSpinning = true;
            _isCharacters = true;
            _needStopWheel = false;

            _currentWinSlot = _isCharacters == false 
                ? SpinHandler.FindSlotForProbability(SpinHandler.slots) 
                : SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);

            if (_currentWinSlot == null)
            {
                return;
            }
            
            _deceleration = SpinHandler.Data.decelerationCharactersWheel;
            _rotationSpeed = SpinHandler.Data.rotationSpeedCharacters;
            
            Debug.Log("Character slot is null? " + _currentWinSlot.name);
            
            if (_currentWinSlot != null) SpinHandler.giftSpinPush.InitializeTransitionParent(0, new DataParentImage(_currentWinSlot.Image, null));
            
            _winSlotCol = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();
            _winSlotColCharacter = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();

            //lockedConversationIsFound = _isCharacters && previewer.FindLockedConversation(_currentWinSlot.GetCurrentIcon());

            EventManager.UpdateTextExperience();

            Debug.Log("winSlot: " + _currentWinSlot.Data.name);
        }

        private void SetSpinWheelSlots()
        {
            SpinHandler.scrollCharactersContent.localPosition = new Vector2(0, int.MaxValue);
            SpinHandler.charactersLockedSlot.gameObject.Activate();
            characterName.gameObject.Deactivate();

            if (!_isSpinning)
            {
                _isSpinning = true;

                _currentWinSlot = _isCharacters == false ? SpinHandler.FindSlotForProbability(SpinHandler.slots) : SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);
                _winSlotCol = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                _winSlotColCharacter = _currentWinSlot.gameObject.GetComponent<BoxCollider2D>();

                _rotationSpeed = SpinHandler.Data.rotationSpeed;

                SpinHandler.giftSpinPush.SetDataParent(new DataParentImage(_currentWinSlot.Image, nameof(_currentWinSlot)));
                
                SpinHandler.giftSpinPush.MakeTransitions();

                Debug.Log("winSlot: " + _currentWinSlot.Data.name);
                
                //_targetRotation = 360 * Random.Range(3, 6) + (360 / slots.Length) * Random.Range(0, slots.Length);
            }
        }

        private Character FindCharacterByWinSlot()
        {
            foreach (var characterData in SpinHandler.Data.characters)
            {
                if (characterData.name != _currentWinSlot.Data.name) continue;
                
                Debug.Log("Found character data by win slot = <color=red>" + characterData.name + "</color>");

                WheelSlotData foundData = characterData;

                if (foundData == null) return null;

                foreach (var character in SpinHandler.Previewer.Characters)
                {
                    if (character.Data.name == foundData.name) return character;
                }
            }

            Debug.LogError("Can't find character by win slot");
            return null;
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
        
        private void ResetSpinStatus()
        {
            _rotationSpeed = SpinHandler.Data.rotationSpeed;
            _deceleration = Random.Range(SpinHandler.Data.decelerationMin, SpinHandler.Data.decelerationMax);
            _isSpinning = false;
            _needStopWheel = false;
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
    }
}