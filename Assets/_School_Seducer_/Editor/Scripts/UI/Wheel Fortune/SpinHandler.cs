using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat.Refactor;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class SpinHandler : SpinHandlerBase
    {
        [Header("UI elements")]
        [SerializeField] private TextMeshProUGUI textSpin;
        [SerializeField] private TextMeshProUGUI _costSpinText;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Transform wheelTransform;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button disableButton;
        [SerializeField] private RectTransform marker;
        [SerializeField] private RectTransform goToStopCharactersWheel;
        [SerializeField] protected RectTransform goToStopWheel;

        protected WheelSlot CurrentWinSlot;
        
        private WheelSlot _currentWinGift;
        private RectTransform _spinButtonRect;
        private RectTransform _winSlotRect;
        private BoxCollider2D _winSlotCol;
        private BoxCollider2D _winSlotColCharacter;
        private Vector3 _goToStopWheelPosition;
        private Character _winCharacter;

        private float _targetYWinCharacter;
        private float _rotationSpeed;
        private float _deceleration;

        private int _winGiftExp;
        
        private bool _isCharacters;
        private bool _isSpinning;
        private bool _winSlotCollidedStopCol;
        private bool _needStopWheel;
        private bool _needAnimate;

        protected void Awake()
        {
            _spinButtonRect = spinButton.GetComponent<RectTransform>();
            _goToStopWheelPosition = goToStopWheel.transform.position;
            spinButton.AddListener(OnSpinButtonClicked);
        }

        protected void OnDestroy()
        {
            spinButton.RemoveListener(OnSpinButtonClicked);
        }

        private void Start()
        {
            InitParametersData();
            
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

            /*if (_isSpinning && _isCharacters)
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
            }*/
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
            TryBuySpin();

            ResetSpinStatus();
            StartCoroutine(ProcessButtonClicked());
        }

        private IEnumerator ProcessButtonClicked()
        {
            yield return HandleSpinSlots();

            yield return HandleWinCharacter();
        }

        private IEnumerator HandleWinCharacter()
        {
            SetSpinWheelCharacters();

            CharacterData winCharacterData = null;
            
            _targetYWinCharacter = _winSlotRect.anchoredPosition.y - 350;

            SpinHandler.charactersLockedSlot.gameObject.Deactivate(.2f);
            yield return HandleScrollCharacter(_targetYWinCharacter);

            characterName.text = CurrentWinSlot.Data.name.ToUpper();
            characterName.gameObject.Activate(0.5f);

            SpinHandler.giftSpinPush.SetDataParent(new DataParentText(characterName));

                //yield return new WaitForSeconds(1.5f);

            Debug.Log($"Current win slot name is <color=red>{CurrentWinSlot.Data.name}</color>");

            Character winCharacter = FindCharacterByCurrentWinSlot();
            _winCharacter = winCharacter;

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

            //winCharacter.Data.ChangeExperience(_winGiftExp);
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
                    ActionStoryUnlocked();

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
                else
                {
                    ActionGiftGot();
                }

                Debug.Log("Last conversation unlocked? = " + winCharacter.Data.LastConversationAvailable());
            }
            
            if (winCharacterData != null)
            {
                SpinHandler.CheckResetCharacter(winCharacterData);
            }
        }

        private IEnumerator HandleScrollCharacter(float anchoredPositionYCharacter)
        {
            float elapsedTime = 0f;

            while (_isSpinning)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / SpinHandler.Data.durationSpinCharacters);
                
                float newY = Mathf.Lerp(SpinHandler.scrollCharactersContent.anchoredPosition.y, anchoredPositionYCharacter, t * t);
                SpinHandler.scrollCharactersContent.anchoredPosition = new Vector2(SpinHandler.scrollCharactersContent.anchoredPosition.x, newY);

                if (Mathf.Approximately(SpinHandler.scrollCharactersContent.anchoredPosition.y, anchoredPositionYCharacter))
                {
                    _isSpinning = false;
                    _isCharacters = false;

                    yield return new WaitForSeconds(0.5f);
                }

                yield return null;
            }
        }

        private IEnumerator HandleSpinSlots()
        {
            if (SpinHandler.scrollCharactersContent.childCount <= 0)
            {
                SetSpinButtonUnavailableStatus();
                yield break;
            }
            
            ResetSpinStatus();
            
            SetSpinWheelSlots();

            SetSpinButtonNoneStatus();

            _rotationSpeed -= SpinHandler.Data.rotationSlowSpeed * Time.fixedDeltaTime;
            yield return new WaitForSeconds(SpinHandler.Data.secondsSlowSpdSlots);

            yield return new WaitUntil(NeedStopWheel);
            
            yield return new WaitUntil(() => _rotationSpeed <= _deceleration);

            yield return new WaitUntil(CalculateResult);

            _winGiftExp = CurrentWinSlot.Data.GetCostExp();

            yield return new WaitForSeconds(2);

            SetSpinButtonSpinStatus();
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
                characterName.text = CurrentWinSlot.Data.name.ToUpper();
                characterName.gameObject.Activate(0.5f);
                
                SpinHandler.giftSpinPush.SetDataParent(new DataParentText(characterName));

                yield return new WaitForSeconds(1.5f);
                
                Debug.Log($"Current win slot name is <color=red>{CurrentWinSlot.Data.name}</color>");

                Character winCharacter =  FindCharacterByCurrentWinSlot();
                _winCharacter = winCharacter;

                //if (winCharacter == null) yield return new WaitForSeconds(1);

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
                
                //winCharacter.Data.ChangeExperience(_winGiftExp);
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
                        ActionStoryUnlocked();

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
                    else
                    {
                        ActionGiftGot();
                    }
                
                    Debug.Log("Last conversation unlocked? = " + winCharacter.Data.LastConversationAvailable());
                }
            }
            else
                _winGiftExp = CurrentWinSlot.Data.GetCostExp();

            yield return new WaitForSeconds(2);

            SetSpinButtonSpinStatus();

            if (_isCharacters) spinButton.onClick.Invoke();

            if (winCharacterData != null)
            {
                SpinHandler.CheckResetCharacter(winCharacterData);
            }
        }

        protected void SetupWinCharacter(Character winCharacter, WheelSlotData winSlot)
        {
            //if (winCharacter == null) yield return new WaitForSeconds(1);

            //winCharacterData = winCharacter.Data;

            Debug.Log("Found character after extra spin: " + winCharacter.Data.name);

            SpinHandler.Previewer.SetLockedConversation(winCharacter);
            
            //ChatStoryResolver chatStoryResolver = ChatStoryResolver as ChatStoryResolver;

            if (winCharacter.Data.ConversationsUnlocked() == false)
            {
                SpinHandler.Previewer.StoryResolver.SetRolledConversation(winCharacter.Data.LockedConversation);
            }
            
            winCharacter.Data.AddGift(winSlot, SpinHandler.Previewer.CharactersConfig.maxGifts);
            Debug.Log("Win gift: " + winSlot.name + " for character: " + winCharacter.Data.name);
        }
        
        private void SetSpinWheelCharacters()
        {
            EnableSpinning();
            EnableCharactersMode();
            ResetStatusSpin();

            CurrentWinSlot = _isCharacters == false 
                ? SpinHandler.FindSlotForProbability(SpinHandler.slots) 
                : SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);

            SetupExtraCharacters();

            if (CurrentWinSlot == null)
            {
                return;
            }
            
            _deceleration = SpinHandler.Data.decelerationCharactersWheel;
            _rotationSpeed = SpinHandler.Data.rotationSpeedCharacters;
            
            Debug.Log("Character slot is null? " + CurrentWinSlot.name);

            if (CurrentWinSlot != null)
            {
                SpinHandler.giftSpinPush.InitializeTransitionParent(0, new DataParentImage(CurrentWinSlot.Image, null));
                Pushes.giftPush.GetComponent<Push>().InitializeTransitionParent(0, new DataParentImage(CurrentWinSlot.Image, null));
            }

            _winSlotRect = CurrentWinSlot.GetComponent<RectTransform>();
            _winSlotCol = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();
            _winSlotColCharacter = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();

            //lockedConversationIsFound = _isCharacters && previewer.FindLockedConversation(_currentWinSlot.GetCurrentIcon());

            EventManager.UpdateTextExperience();

            Debug.Log("winSlot: " + CurrentWinSlot.Data.name);
        }

        protected virtual void ActionStoryUnlocked() => SpinHandler.ShowUnlockStoryPush();
        protected virtual void ActionGiftGot()
        {
            _winCharacter.Data.AddGift(_currentWinGift.Data, SpinHandler.Previewer.CharactersConfig.maxGifts);
            //SpinHandler.ShowGiftPush();
            Pushes.ShowGiftPush();
        }

        protected virtual void SetupExtraSlots() { }
        protected virtual void SetupExtraCharacters() { }

        private void ResetStatusSpin() => _needStopWheel = false;

        private void EnableCharactersMode() => _isCharacters = true;

        private void EnableSpinning() => _isSpinning = true;

        private void SetSpinWheelSlots()
        {
            SpinHandler.scrollCharactersContent.localPosition = new Vector2(0, int.MaxValue);
            SpinHandler.charactersLockedSlot.gameObject.Activate();
            characterName.gameObject.Deactivate();

            if (!_isSpinning)
            {
                EnableSpinning();

                CurrentWinSlot = _isCharacters == false ? SpinHandler.FindSlotForProbability(SpinHandler.slots) : SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);
                _currentWinGift = CurrentWinSlot;
                _winSlotCol = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                _winSlotColCharacter = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                
                SetupExtraSlots();

                _rotationSpeed = SpinHandler.Data.rotationSpeed;

                //SpinHandler.giftSpinPush.SetDataParent(new DataParentImage(CurrentWinSlot.Image, nameof(CurrentWinSlot)));
                SpinHandler.giftSpinPush.InitializeData(new TransitionDataSprite(CurrentWinSlot.Data.iconInfo, nameof(CurrentWinSlot)));
                Pushes.giftPush.GetComponent<Push>().InitializeData(new TransitionDataSprite(CurrentWinSlot.Data.iconInfo, nameof(CurrentWinSlot)));
                
                SpinHandler.giftSpinPush.MakeTransitions();

                Debug.Log("winSlot: " + CurrentWinSlot.Data.name);
                
                //_targetRotation = 360 * Random.Range(3, 6) + (360 / slots.Length) * Random.Range(0, slots.Length);
            }
        }

        private Character FindCharacterByCurrentWinSlot()
        {
            foreach (var characterData in SpinHandler.Data.characters)
            {
                if (characterData.name != CurrentWinSlot.Data.name) continue;
                
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
            disableButton.interactable = false;
        }

        private void SetSpinButtonSpinStatus()
        {
            textSpin.text = "SPIN!";
            _costSpinText.gameObject.Activate();
            spinButton.interactable = true;
            closeButton.interactable = true;
            disableButton.interactable = true;
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