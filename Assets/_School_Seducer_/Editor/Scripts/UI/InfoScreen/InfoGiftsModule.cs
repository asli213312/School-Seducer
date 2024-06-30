using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.UI;
using _School_Seducer_.Editor.Scripts.UI.Popups;
using _School_Seducer_.Editor.Scripts.Services;
using _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune;
using _School_Seducer_.Editor.Scripts.Utility;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoGiftsModule : MonoBehaviour, IModule<InfoScreenSystem>, ICharacterSelected
    {
        [Inject] private SpineUtility _spineUtility;
        [Inject] private SaveToDB _saver;
        [Inject] private MonoController _monoController;
        
        [Header("Data")]
        [SerializeField] private CharactersConfig config;
        [SerializeField] private GiftView giftPrefab;

        [Header("UI")] 
        [SerializeField] private Notificator notificator;
        [SerializeField] private TextMeshProUGUI expSliderFormulaText;
        [SerializeField] private Slider barExpCharacter;
        [SerializeField] private Slider infoExpBar;
        [SerializeField] private Transform content;
        [SerializeField] private Button presentButton;
        [SerializeField] private EventTrigger spinTrigger;
        
        [Header("Pushes")]
        [SerializeField] private Push unlockedStoryPush;
        [SerializeField] private Push giftPresentPush;

        [Header("Options")]
        [SerializeField] private float progressBarSpeed = 50f;

        public event Action OnPresentAction;
        public List<GiftView> Gifts { get; private set; } = new();
        private InfoScreenSystem _system;
        private Character _currentCharacter;
        public int ScoreCurrentGifts { get; private set; }

        private int _lockedConversationScore;
        private int _characterExpBeforePresent;

        private bool _needChangeSliderState;
        
        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }

        public void Initialize()
        {
            _system.Previewer.CharacterSelectedEvent += OnCharacterSelected;
            presentButton.AddListener(PresentGift);
            presentButton.AddListener(() => OnPresentAction?.Invoke());
        }

        public void OnCharacterSelected(Character character)
        {
        	if (_currentCharacter != null && _currentCharacter != character)
                ResetContent();

            _currentCharacter = character;
            
            _system.Previewer.SetLockedConversation(_currentCharacter);

            if (_currentCharacter.Data.LockedConversation != null)
            {
                _lockedConversationScore = _currentCharacter.Data.LockedConversation.costExp;   
            }
            
            _characterExpBeforePresent = _currentCharacter.Data.experience;

            SetStatusContent(character);

            SetCalculatedFormulaOnExpBar();
        }

        private void SetStatusContent(Character character)
        {
            if (character.Data.gifts.Count > 0)
            {
                presentButton.gameObject.Activate();        
                RenderContent(character);
            }
            else
            {
                ScoreCurrentGifts = 0;
                presentButton.gameObject.Deactivate();
            }
        }

        public void Reset()
        {
            ResetContent();
            ResetConsumedGifts();
            
            SetStatusContent(_currentCharacter);
        }

        private void PresentGift()
        {
            if (ScoreCurrentGifts == 0) return;
            if (_currentCharacter.Data.LastConversationAvailable()) return;
            
            _system.StoryCounter.ResetAmountToStoryBar();
            
            giftPresentPush.InitializeData(new TransitionDataSprite(_currentCharacter.Data.info.portrait, "CharacterPortrait"));
            giftPresentPush.InitializeData(new TransitionDataText(_currentCharacter.Data.name, "characterName"));

            _spineUtility.InvokeStartAnimation(giftPresentPush.GetComponent<SkeletonGraphic>());

            StartCoroutine(IterateBarStates(barExpCharacter, () =>
            {
	            _system.Previewer.OnCharacterSelected(_currentCharacter);
	            
	            _system.Previewer.StoryResolver.SetSlider(infoExpBar);
	            _system.Previewer.StoryResolver.SetSliderValue(_currentCharacter.Data.experience);
            }));
        }

        public void InvokeSlidersAnimate() 
        {
        	StartCoroutine(InstallExpBarAnimated(barExpCharacter, _currentCharacter));
        }

        private void InstallExpBar(Slider bar, Character character)
        {
            int characterExpAfterPresent = ScoreCurrentGifts;
            Debug.Log("LockedConversation exp while installing expBar: " + _lockedConversationScore);

            if (ScoreCurrentGifts + _characterExpBeforePresent >= _lockedConversationScore) 
            {
                characterExpAfterPresent = ScoreCurrentGifts - (_lockedConversationScore - _characterExpBeforePresent);
                _needChangeSliderState = true;
                Debug.Log("ScoreCurrentGifts > characterExp = " + character.Data.experience);
            }
            else 
            {
                characterExpAfterPresent = ScoreCurrentGifts + _characterExpBeforePresent;
                Debug.Log("ScoreCurrentGifts < characterExp = " + character.Data.experience);
            }

            character.Data.experience = characterExpAfterPresent;
            Debug.Log("Exp after present: " + characterExpAfterPresent);
            
            _system.Previewer.SetLockedConversation(character);
            _system.Previewer.StoryResolver.SetLockedConversation(character.Data.LockedConversation);
            _system.Previewer.StoryResolver.UpdateStatusViews();
            _system.Previewer.StoryResolver.SetSlider(bar);
            _system.Previewer.StoryResolver.SetSliderValue(1);
            _system.Previewer.StoryResolver.SetSliderValue(ScoreCurrentGifts);

            SetCalculatedFormulaOnExpBar();
        }

        private IEnumerator InstallExpBarAnimated(Slider bar, Character character)
        {
        	InstallExpBar(infoExpBar, _currentCharacter);
			_system.Previewer.StoryResolver.SetSlider(infoExpBar);
            _system.Previewer.StoryResolver.SetSliderValue(_currentCharacter.Data.experience);
            //_system.Previewer.StoryResolver.SetSlider(barExpCharacter);
            //_system.Previewer.StoryResolver.SetSliderValue(_currentCharacter.Data.experience);
            _system.Previewer.StoryResolver.SetSlider(_system.StoryCounter.GetMainBar()); 
            _system.Previewer.StoryResolver.SetSliderValue(_currentCharacter.Data.experience);
            
            _system.Previewer.SetLockedConversation(_currentCharacter);            

            int characterExpAfterPresent = ScoreCurrentGifts;
            Debug.Log("LockedConversation exp while installing expBar: " + _lockedConversationScore);

            if (ScoreCurrentGifts + _characterExpBeforePresent >= _lockedConversationScore) 
            {
                characterExpAfterPresent = ScoreCurrentGifts - (_lockedConversationScore - _characterExpBeforePresent);
                Debug.Log("ScoreCurrentGifts > characterExp = " + character.Data.experience);
            }
            else 
            {
                characterExpAfterPresent = ScoreCurrentGifts + _characterExpBeforePresent;
                Debug.Log("ScoreCurrentGifts < characterExp = " + character.Data.experience);
            }

            character.Data.experience = characterExpAfterPresent;
            Debug.Log("Exp after present: " + characterExpAfterPresent);

            _system.Previewer.SetLockedConversation(character);
            _system.Previewer.StoryResolver.SetSlider(bar);
            _system.Previewer.StoryResolver.SetLockedConversation(character.Data.LockedConversation);

            _system.Previewer.StoryResolver.UpdateStatusViews();            

            if (_needChangeSliderState) 
            {
            	yield return new WaitForSeconds(1f);
            	StartCoroutine(bar.AnimateProgression(_currentCharacter.Data.LockedConversation.costExp, 1.5f, () =>
            	{
            		_system.Previewer.StoryResolver.SetSliderValue(0);
            		StartCoroutine(bar.AnimateProgression(_currentCharacter.Data.experience, 1.5f));
            	}));
            }
            else 
            {
            	yield return new WaitForSeconds(1f);
            	yield return bar.AnimateProgression(_currentCharacter.Data.experience, 1.5f);
            }

            _system.Previewer.StoryResolver.UpdateStatusViews();
            _system.Previewer.StoryResolver.SetSlider(bar);

            SetCalculatedFormulaOnExpBar();

            _needChangeSliderState = false;
             ScoreCurrentGifts = 0;
        }

        private IEnumerator IterateBarStates(Slider bar, Action onFinished) 
        {
        	int allExp = ScoreCurrentGifts + _characterExpBeforePresent;
            List<SliderState> states = new();
            bool needEnd = false;
            bool showUnlockStoryPush = false;

            Debug.Log("<color=green>BEGINS</color> ITERATOR STATES");
        	Debug.Log("-------------------------------------------");
            
        	_currentCharacter.Data.ChangeExperience(ScoreCurrentGifts);

            _system.Previewer.SetLockedConversation(_currentCharacter);
            _system.Previewer.StoryResolver.SetSlider(bar);
            _system.Previewer.StoryResolver.SetSliderValue(_characterExpBeforePresent);

        	foreach (var conversation in _currentCharacter.Data.allConversations) 
        	{
	            if (needEnd) break;
	            
	            _system.Previewer.SetLockedConversation(_currentCharacter);
	            
	            if (conversation != _currentCharacter.Data.LockedConversation) continue;
	            
        		bool storyUnlocked = false;

                if (allExp >= conversation.costExp) 
        		{
        			allExp -= conversation.costExp;
        			storyUnlocked = true;
        		}

                if (_currentCharacter.Data.allConversations[^1].isUnlocked == false)
	            {
	                if (storyUnlocked)
	                {
		                showUnlockStoryPush = true;
		                
	                    if (_currentCharacter.Data.LockedConversation != _currentCharacter.Data.allConversations[^1])
	                    {
	                        _currentCharacter.Data.LockedConversation.isUnlocked = true;
	                        _currentCharacter.Data.LockedConversation.isCompleted = false;
	                        _currentCharacter.Data.LockedConversation.isSeen = false;
	                        _currentCharacter.Data.experience = allExp;
	                    }

	                    if (_currentCharacter.Data.allConversations[^1] == _currentCharacter.Data.LockedConversation)
	                    {
	                        _currentCharacter.Data.allConversations[^1].isUnlocked = true;
	                        _currentCharacter.Data.allConversations[^1].isCompleted = false;
	                        _currentCharacter.Data.allConversations[^1].isSeen = false;
	                    }
	                    
	                    notificator.Notify();

	                    bar.maxValue = _currentCharacter.Data.LockedConversation.costExp;
	                    _system.Previewer.StoryResolver.SetSliderValue(_characterExpBeforePresent);

	                    states.Add(new SliderState
	                    {
		                    MaxValue = bar.maxValue,
		                    CurrentValue = _characterExpBeforePresent,
		                    NextValue = bar.maxValue
	                    });

	                    _system.Previewer.SetLockedConversation(_currentCharacter);

	                    if (allExp >= _currentCharacter.Data.LockedConversation.costExp)
	                    {
		                    _characterExpBeforePresent = 0;
		                    continue;
	                    }
	                    
	                    bar.maxValue = _currentCharacter.Data.LockedConversation.costExp;
	                    _system.Previewer.StoryResolver.SetSliderValue(0);

	                    states.Add(new SliderState
	                    {
		                    MaxValue = _currentCharacter.Data.LockedConversation.costExp,
		                    CurrentValue = 0,
		                    NextValue = allExp
	                    });

	                    needEnd = true;
	                    break;
	                }
	                else
	                {
		                bar.maxValue = _currentCharacter.Data.LockedConversation.costExp;
		                _system.Previewer.StoryResolver.SetSliderValue(_characterExpBeforePresent);
		                
		                states.Add(new SliderState
		                {
			                MaxValue = bar.maxValue,
			                CurrentValue = _characterExpBeforePresent,
			                NextValue = allExp
		                });
		                
		                showUnlockStoryPush = false;

		                needEnd = true;
		                break;
	                }
	            }
            }
            
            onFinished?.Invoke();

            // ShowGiftPush();
            // yield return new WaitForSeconds(1.5f);
            // foreach (var state in states)
            // {
	           //  bar.maxValue = state.MaxValue;
	           //  _system.Previewer.StoryResolver.SetSliderValue(state.CurrentValue);
	           //  yield return bar.AnimateProgression(state.NextValue, 1.5f);
	           //  yield return new WaitForSeconds(0.5f);
            // }

            if (showUnlockStoryPush)
            {
            	Debug.Log("State[0].CurrentValue storyUnlocked: " + states[0].CurrentValue);

				bar.maxValue = states[0].MaxValue;
				bar.value = 0;
				bar.value = states[0].CurrentValue;

            	ShowGiftPush();
	            yield return new WaitForSeconds(1.5f);
	            yield return bar.AnimateProgressionBySpeed(bar.maxValue, progressBarSpeed);
	            yield return new WaitUntil(() => giftPresentPush.gameObject.activeSelf == false);

	            unlockedStoryPush.InitializeData(new TransitionDataSprite(_currentCharacter.Data.info.portrait, "CharacterPortrait"));
	            unlockedStoryPush.InitializeData(new TransitionDataText(_currentCharacter.Data.name, "characterName"));

	            ShowUnlockStory();
            }
            else
            {
	            ShowGiftPush();
	            yield return new WaitForSeconds(1.5f);
	            
	            bar.maxValue = states[0].MaxValue;
	            _system.Previewer.StoryResolver.SetSliderValue(states[0].CurrentValue);
	            yield return bar.AnimateProgressionBySpeed(states[0].NextValue, progressBarSpeed);
            }

            spinTrigger.gameObject.Activate();

            SetStatusContent(_currentCharacter);

            _characterExpBeforePresent = _currentCharacter.Data.experience;
            ScoreCurrentGifts = 0;

            Debug.Log("-------------------------------------------");
        	Debug.Log("<color=red>FINISHED</color> ITERATOR STATES");
        }

        private void SetCalculatedFormulaOnExpBar() 
        {
            int currentExp = _currentCharacter.Data.experience;
            int willBeAddedExp = ScoreCurrentGifts;
            int needExp = _currentCharacter.Data.LockedConversation.costExp;

            string selectedText = "";

            if (ScoreCurrentGifts == 0)
            	selectedText = $"{currentExp}/{needExp}";
            else 
            	selectedText = $"{currentExp}(+{willBeAddedExp})/{needExp}";

            expSliderFormulaText.text = selectedText;
        }

        private void ShowGiftPush()
        {
            StartCoroutine(giftPresentPush.transform.DoLocalScaleAndUnscale(this, giftPresentPush.transform.localScale));
        }

        private void ShowUnlockStory()
        {
            StartCoroutine(unlockedStoryPush.transform.DoLocalScaleAndUnscale(this, new Vector3(0.008f, 0.008f, 0.008f)));
        }

        private void RenderContent(Character character)
        {
            ResetContent();

            ScoreCurrentGifts = 0;

            List<WheelSlotData> existsDataSlots = new();

            for (int i = 0; i < character.Data.gifts.Count; i++)
            {
                var giftData = character.Data.gifts[i];
                
                if (i > config.maxGifts - 1) break;
                
                ScoreCurrentGifts += giftData.score;

                if (existsDataSlots.Contains(giftData) == false)
                    existsDataSlots.Add(giftData);
                else continue;
                
                var giftView = Instantiate(giftPrefab, content);
                giftView.Render(giftData, FindSameGiftCountOfCharacter(character.Data, giftData));
            }

            _monoController.UpdateMonoByName("EarnScoreGifts");
            Debug.Log("Rendered all score gifts: " + ScoreCurrentGifts);
        }

        private int FindSameGiftCountOfCharacter(CharacterData characterData, WheelSlotData giftData)
        {
            return characterData.gifts.Where(x => x == giftData).ToArray().Length;
        }

        private void ResetContent()
        {
	        if (content.childCount <= 0) return;
	        
	        for (int i = 0; i < content.childCount; i++)
	        {
		        Destroy(content.GetChild(i).gameObject);
	        }
        }

        private void ResetConsumedGifts()
        {
            if (_currentCharacter != null && _currentCharacter.Data.gifts.Count > 0)
            {
                _currentCharacter.Data.gifts.Clear();
                ScoreCurrentGifts = 0;
                
                //_saver.mainData.AddCharacterData(_currentCharacter.Data);

                //_saver.SAVE();
            }
            
            Gifts.Clear();

            SetCalculatedFormulaOnExpBar();
        }

        private void OnDestroy()
        {
            _system.Previewer.CharacterSelectedEvent -= OnCharacterSelected;
            presentButton.RemoveListener(PresentGift);
            presentButton.RemoveListener(() => OnPresentAction?.Invoke());
        }
        
        private class SliderState
        {
	        public float MaxValue;
	        public int CurrentValue;
	        public float NextValue;
        }
    }
}