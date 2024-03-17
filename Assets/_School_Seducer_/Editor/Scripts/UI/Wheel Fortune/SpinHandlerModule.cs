using System;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using NaughtyAttributes;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class SpinHandlerModule : MonoBehaviour, IModule<WheelFortuneSystem>
    {
        [Inject] protected Bank Bank;
        [Inject] protected EventManager EventManager;
        [Inject] protected IChatStoryResolverModule ChatStoryResolver;
        
        [Header("Handlers")]
        [SerializeField] private SpinHandler defaultSpinHandler;
        [SerializeField] private ExtraSpinHandler extraSpinHandler;
        [SerializeField] private SpinHandlerBase uniqueSpinHandler;

        [Header("UI elements")] 
        [SerializeField] public RectTransform charactersLockedSlot;
        [SerializeField] private Slider expSlider;
        [SerializeField] public RectTransform scrollCharactersContent;
        [SerializeField] public WheelSlot characterSlotPrefab;
        [SerializeField] public List<WheelSlot> slots;
        
        [Header("PushUps")] 
        [SerializeField] public Push giftSpinPush;
        [SerializeField] public Push unlockNewStoryPush;

        public Previewer Previewer => _system.Previewer;
        public WheelFortuneData Data => _system.Data;
        
        [field: ShowInInspector] public List<WheelSlot> CharacterSlots {get; set; } = new();
        
        private WheelFortuneSystem _system;

        public void InitializeCore(WheelFortuneSystem system)
        {
            _system = system;
        }

        public void Initialize()
        {
            defaultSpinHandler.InitializeCore(this);
            extraSpinHandler.InitializeCore(this);
            uniqueSpinHandler.InitializeCore(this);
            
            defaultSpinHandler.Initialize(_system.PushesModule);
            extraSpinHandler.Initialize(_system.PushesModule);
            uniqueSpinHandler.Initialize(_system.PushesModule);
        }

        private void Awake()
        {
            AddListenerToOpenChatButton();
            void AddListenerToOpenChatButton()
            {
                unlockNewStoryPush.Buttons[0].AddListener(() =>
                {
                    unlockNewStoryPush.gameObject.Deactivate();
                    gameObject.Deactivate(1);
                    _system.Previewer.OnCharacterSelected(_system.Previewer.CurrentCharacter);
                });
            }
        }

        private void OnDestroy()
        {
            RemoveListenerToOpenChatButton();
            void RemoveListenerToOpenChatButton()
            {
                unlockNewStoryPush.Buttons[0].RemoveListener(() =>
                {
                    unlockNewStoryPush.gameObject.Deactivate();
                    gameObject.Deactivate(1);
                    _system.Previewer.OnCharacterSelected(_system.Previewer.CurrentCharacter);
                });
            }
        }

        private void Start()
        {
            InitializeSlots();
            
            //CheckResetCharactersOutWheel();

            EventManager.UpdateTextMoney();

            void CheckResetCharactersOutWheel()
            {
                _system.Previewer.Characters.ForEach(character => CheckResetCharacter(character.Data));
            }
        }
        
        public void ShowGiftPush()
        {
            ShowPush(giftSpinPush, 0.95f);
        }

        public void ShowUnlockStoryPush()
        {
            ShowPush(unlockNewStoryPush, 0.9f);
        }

        public void ShowPush(Push push, float scaleMultiplier)
        {
            StartCoroutine(push.transform.DoLocalScaleAndUnscale(this, new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier)));
        }
        
        public void OnChangeValueExperience(int newValue)
        {
            expSlider.value = newValue;
            Debug.Log($"Slider value changed to: {newValue}");
        }
        
        public WheelSlot FindSlotForProbability(List<WheelSlot> wheelSlots)
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

        public void CheckResetCharacter(CharacterData characterData)
        {
            //foreach (var conversation in characterData.allConversations)
           // {
                //if (conversation.isUnlocked == false) return;
            //}

            for (int i = 0; i < scrollCharactersContent.childCount; i++)
            {
                WheelSlot slotCharacterToReset = scrollCharactersContent.GetChild(i).GetComponent<WheelSlot>();

                if (slotCharacterToReset.Data.name == characterData.name)
                {
                    CharacterSlots.Remove(slotCharacterToReset);
                    slotCharacterToReset.gameObject.Destroy();
                }
            }
        }

        private void InitializeSlots()
        {
            int charactersCount = _system.Data.characters.Count;
            int targetNumberOfSlots = 3 * 8;

            for (int i = 0; i < targetNumberOfSlots; i++)
            {
                var character = _system.Data.characters[i % charactersCount];
                WheelSlot characterSlot = Instantiate(characterSlotPrefab, scrollCharactersContent);
                characterSlot.Initialize(character, _system.Data.iconMoney);
                
                if (i < Data.characters.Count)
                {
                    CharacterSlots.Add(characterSlot);
                }
            }

            scrollCharactersContent.localPosition = new Vector2(0, int.MaxValue);

            foreach (var slot in slots)
            {
                slot.Initialize(slot.Data, _system.Data.iconMoney);
            }
        }
    }
}