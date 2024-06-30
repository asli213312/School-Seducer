using System.Collections;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat.Refactor;
using _School_Seducer_.Editor.Scripts.Extensions;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using _School_Seducer_.Editor.Scripts.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class SpinHandler : SpinHandlerBase
    {
        [Inject] private SpinHandlerUtility _spinHandlerUtility;
        
        [Header("UI elements")]
        [SerializeField] private TextMeshProUGUI textSpin;
        [SerializeField] protected TextMeshProUGUI _costSpinText;
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
        protected float RotationSpeed;
        protected float Deceleration;

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

        protected virtual void Start()
        {
            InitParametersData();
            
            _costSpinText.text = SpinHandler.Data.moneyForSpin.ToString();
        }
        
        protected virtual void InitParametersData()
        {
            RotationSpeed = SpinHandler.Data.rotationSpeed;

            Deceleration = Random.Range(SpinHandler.Data.decelerationMin, SpinHandler.Data.decelerationMax);
        }

        protected override void Spin()
        {
            if (_isSpinning && ConditionToSpinning())
            {
                float step = RotationSpeed * Time.fixedDeltaTime;
                wheelTransform.Rotate(Vector3.forward * step);

                _winSlotCollidedStopCol = _winSlotCol.OverlapPoint(_goToStopWheelPosition);
                
                if (_needStopWheel)
                {
                    if (RotationSpeed > 0)
                    {
                        RotationSpeed -= Deceleration * Time.fixedDeltaTime;
                    }
                    else
                    {
                        _isSpinning = false;
                        _isCharacters = true;
                        CalculateResult();
                    }
                }
            }
        }

        protected virtual bool ConditionToSpinning() => !_isCharacters;

        protected override void Stop() { }
        
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
            bool enoughMoney = TryBuySpin();

            if (enoughMoney == false)
            {
                return;
            }

            SpinHandler.spinStarted?.Invoke();

            ResetSpinStatus();
            StartCoroutine(ProcessButtonClicked());
        }

        protected virtual IEnumerator ProcessButtonClicked()
        {
            yield return HandleSpinSlots();

            yield return HandleWinCharacter();
            
            SetSpinButtonSpinStatus();

            SpinHandler.spinCompleted?.Invoke();
        }

        protected IEnumerator HandleWinCharacter()
        {
            SetSpinWheelCharacters();

            characterName.text = CurrentWinSlot.Data.name.ToUpper();
            this.Activate(characterName.transform, 0.5f);

            //SpinHandler.giftSpinPush.SetDataParent(new DataParentText(characterName));

                //yield return new WaitForSeconds(1.5f);

            Debug.Log($"Current win slot name is <color=red>{CurrentWinSlot.Data.name}</color>");

            Character winCharacter = FindCharacterByCurrentWinSlot();
            _winCharacter = winCharacter;

            InvokeCharactersMode();

            // for (int i = 0; i < SpinHandler.Previewer.Characters.Length; i++)
            // {
            //     if (winCharacter.Data != SpinHandler.Previewer.Characters[i].Data) continue;
            //     
            //     if (i == 0) _targetYWinCharacter = _winSlotRect.anchoredPosition.y + 350;
            //     else if (i == 1) _targetYWinCharacter = _winSlotRect.anchoredPosition.y;
            //     else _targetYWinCharacter = _winSlotRect.anchoredPosition.y - 350;
            // }
            
            CharacterData winCharacterData = null;
            
            this.Deactivate(SpinHandler.charactersLockedSlot, 0.2f);
            yield return InvokeHandleScrollCharacter();

            winCharacterData = winCharacter.Data;

            SpinHandler.Previewer.CurrentCharacter = winCharacter;

            Debug.Log("Found character after spin: " + winCharacter.Data.name);

            int storiesUnlockedCount = winCharacterData.allConversations.Count(x => x.isUnlocked);
            Pushes.giftPush.GetComponent<Push>().InitializeData(new TransitionDataText(storiesUnlockedCount.ToString(), "StoriesUnlocked_Count"));
            
            SpinHandler.Previewer.SetLockedConversation(winCharacter);
            
            SpinHandler.Previewer.StoryResolver.UpdateStatusViews();

            ChatStoryResolver chatStoryResolver = ChatStoryResolver as ChatStoryResolver;

            if (winCharacter.Data.ConversationsUnlocked() == false)
            {
                SpinHandler.Previewer.StoryResolver.SetLockedConversation(winCharacter.Data.LockedConversation);
                chatStoryResolver.SetRolledConversation(winCharacter.Data.LockedConversation);
            }

            SpinHandler.Previewer.StoryResolver.SetSlider(Pushes.giftPush.GetComponent<Push>().GetBarByIndex(0));
            
            SpinHandler.Previewer.StoryResolver.SetSliderValue(winCharacter.Data.experience);
            
            SpinHandler.Previewer.StoryResolver.UpdateStatusViews();
            
            SpinHandler.Previewer.StoryResolver.SetLockedConversation(winCharacter.Data.LockedConversation);

            SpinHandler.Previewer.StoryResolver.SetSliderValue(winCharacter.Data.experience);

            SpinHandler.Previewer.StoryResolver.UpdateStatusViews();
            SpinHandler.Previewer.StoryResolver.UpdateStatusViews();
            ChatStoryResolver.UpdateView();

            EventManager.UpdateTextExperience();

            SpinHandler.Previewer.StoryResolver.UpdateStatusViews();
            ChatStoryResolver.UpdateView();

            EventManager.UpdateTextExperience();
            
            ActionGiftGot();

            this.Activate(SpinHandler.charactersLockedSlot, 0.2f);
            this.Deactivate(characterName.transform, 0.5f);
        }

        protected virtual void InvokeCharactersMode() { }

        protected virtual IEnumerator InvokeHandleScrollCharacter()
        {
            yield return HandleScrollCharacter();
        }

        protected IEnumerator HandleScrollCharacter()
        {
            float elapsedTime = 0f;

            while (_isSpinning)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / SpinHandler.Data.durationSpinCharacters);

                float newY = Mathf.Lerp(SpinHandler.scrollCharactersContent.anchoredPosition.y, -CurrentWinSlot.transform.localPosition.y + goToStopCharactersWheel.localPosition.y, t * t);
                SpinHandler.scrollCharactersContent.anchoredPosition = new Vector2(SpinHandler.scrollCharactersContent.anchoredPosition.x, newY);

                float threshold = 0.1f;

                if (Mathf.Abs(SpinHandler.scrollCharactersContent.anchoredPosition.y + CurrentWinSlot.transform.localPosition.y - goToStopCharactersWheel.localPosition.y) <= threshold)
                {
                    _isSpinning = false;
                    _isCharacters = false;

                    yield return new WaitForSeconds(0.5f);
                }

                yield return null;
            }
        }

        protected IEnumerator HandleSpinSlots()
        {
            if (SpinHandler.scrollCharactersContent.childCount <= 0)
            {
                SetSpinButtonUnavailableStatus();
                yield break;
            }
            
            ResetSpinStatus();
            
            SetSpinWheelSlots();

            SetSpinButtonNoneStatus();

            SubtractSpeedSlots();

            yield return WaitForResult();
        }

        protected virtual IEnumerator WaitForResult()
        {
            yield return new WaitForSeconds(SpinHandler.Data.secondsSlowSpdSlots);
            
            yield return new WaitUntil(NeedStopWheel);
            
            yield return new WaitUntil(() => RotationSpeed <= Deceleration);

            yield return new WaitUntil(CalculateResult);

            yield return new WaitForSeconds(2);
        }

        protected virtual void SubtractSpeedSlots()
        {
            RotationSpeed -= SpinHandler.Data.rotationSlowSpeed * Time.fixedDeltaTime;
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
                RotationSpeed -= SpinHandler.Data.rotationSlowSpeed * Time.fixedDeltaTime;
                yield return new WaitForSeconds(SpinHandler.Data.secondsSlowSpdSlots);
            }

            yield return new WaitUntil(NeedStopWheel);
            
            yield return new WaitUntil(() => RotationSpeed <= Deceleration);

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
                    SpinHandler.Previewer.StoryResolver.SetLockedConversation(winCharacter.Data.LockedConversation);
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
                //SpinHandler.CheckResetCharacter(winCharacterData);
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
                SpinHandler.Previewer.StoryResolver.SetLockedConversation(winCharacter.Data.LockedConversation);
            }
            
            winCharacter.Data.AddGift(winSlot, SpinHandler.Previewer.CharactersConfig.maxGifts);
            
            Saver.mainData.AddCharacterData(winCharacter.Data);
            Saver.SAVE();
            
            Debug.Log("Win gift: " + winSlot.name + " for character: " + winCharacter.Data.name);
        }
        
        private void SetSpinWheelCharacters()
        {
            EnableSpinning();
            EnableCharactersMode();
            ResetStatusSpin();

            CurrentWinSlot = SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);
            
            if (_spinHandlerUtility.SelectedWinCharacter != null) 
                CurrentWinSlot = _spinHandlerUtility.SelectedWinCharacter;

            SetupExtraCharacters();

            if (CurrentWinSlot == null)
            {
                return;
            }
            
            SetupSpeedParametersCharacters();

            Debug.Log("Character slot is null? " + CurrentWinSlot.name);

            if (CurrentWinSlot != null)
            {
                SpinHandler.giftSpinPush.InitializeTransitionParent(0, new DataParentImage(CurrentWinSlot.Image, null));
                Pushes.giftPush.GetComponent<Push>().InitializeTransitionParent(0, new DataParentImage(CurrentWinSlot.Image, null));
                Pushes.giftPush.GetComponent<Push>().InitializeData(new TransitionDataText(CurrentWinSlot.Data.name, "characterName"));

                SpinHandler.Previewer.StoryResolver.SetSlider(Pushes.giftPush.GetComponent<Push>().GetBarByIndex(0));
            }

            _winSlotRect = CurrentWinSlot.GetComponent<RectTransform>();
            _winSlotCol = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();
            _winSlotColCharacter = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();

            //lockedConversationIsFound = _isCharacters && previewer.FindLockedConversation(_currentWinSlot.GetCurrentIcon());

            EventManager.UpdateTextExperience();

            Debug.Log("winSlot: " + CurrentWinSlot.Data.name);
        }

        protected virtual void SetupSpeedParametersCharacters()
        {
            Deceleration = SpinHandler.Data.decelerationCharactersWheel;
            RotationSpeed = SpinHandler.Data.rotationSpeedCharacters;
        }

        protected virtual void ActionStoryUnlocked() => SpinHandler.ShowUnlockStoryPush();
        protected virtual void ActionGiftGot()
        {
            _winCharacter.Data.AddGift(_currentWinGift.Data, SpinHandler.Previewer.CharactersConfig.maxGifts);
            Saver.mainData.AddCharacterData(_winCharacter.Data);
            Saver.SAVE();
            //SpinHandler.ShowGiftPush();
            Pushes.ShowGiftPush();
        }

        protected virtual void SetupExtraSlots()
        {
            RotationSpeed = SpinHandler.Data.rotationSpeed;
        }
        protected virtual void SetupExtraCharacters() { }

        protected void DisableCharactersMode() => _isCharacters = false;
        protected void DisableSpinning() => _isSpinning = false;

        protected void InvokeStopWheel() => _needStopWheel = true;
        private void ResetStatusSpin() => _needStopWheel = false;

        protected void EnableCharactersMode() => _isCharacters = true;

        private void EnableSpinning() => _isSpinning = true;

        private void SetSpinWheelSlots()
        {
            SpinHandler.scrollCharactersContent.localPosition = new Vector2(0, int.MaxValue);
            characterName.gameObject.Deactivate();

            if (!_isSpinning)
            {
                EnableSpinning();

                CurrentWinSlot = _isCharacters == false ? SpinHandler.FindSlotForProbability(SpinHandler.slots) : SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);
                _currentWinGift = CurrentWinSlot;
                _winSlotCol = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                _winSlotColCharacter = CurrentWinSlot.gameObject.GetComponent<BoxCollider2D>();
                
                SetupExtraSlots();

                //SpinHandler.giftSpinPush.SetDataParent(new DataParentImage(CurrentWinSlot.Image, nameof(CurrentWinSlot)));
                SpinHandler.giftSpinPush.InitializeData(new TransitionDataSprite(CurrentWinSlot.Data.iconInfo, nameof(CurrentWinSlot)));
                Pushes.giftPush.GetComponent<Push>().InitializeData(new TransitionDataSprite(CurrentWinSlot.Data.iconInfo, nameof(CurrentWinSlot)));
                Pushes.giftPush.GetComponent<Push>().InitializeData(new TransitionDataText("+" + CurrentWinSlot.Data.score, "slotScore"));

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

        protected void SetSpinButtonSpinStatus()
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
        
        protected void ResetSpinStatus()
        {
            ResetSpeedParameters();
            _isSpinning = false;
            _needStopWheel = false;
        }

        protected virtual void ResetSpeedParameters()
        {
            RotationSpeed = SpinHandler.Data.rotationSpeed;
            Deceleration = Random.Range(SpinHandler.Data.decelerationMin, SpinHandler.Data.decelerationMax);
        }

        private bool CalculateResult()
        {
            RaycastHit2D hit = GetRaycastDownMarker();
            
            //if (_isCharacters == false) _isCharacters = true;

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Map")) return false;

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