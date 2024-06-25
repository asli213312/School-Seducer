using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Services;
using GameAnalyticsSDK;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Nutaku;
using Nutaku.Unity;
using UnityEngine.Events;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Previewer : MonoBehaviour
    {
        [Inject] private SaveToDB _saver;
        [Inject] private EventManager _eventManager;
        [Inject] private Bank _bank;
        [Inject] private IChatInitialization _chatInitializationModule;
        
        [Header("UI Data")] 
        [SerializeField] private GameObject previewerPanel;
        [SerializeField] private ChatSystem chatSystem;

        [Header("Data")] 
        [SerializeField] private CharactersConfig charactersConfig;
	    [SerializeField] private Chat.Chat _chat;
        [SerializeField] private StoryResolver storyResolver;
        [SerializeField] private LevelChecker levelChecker;
        [SerializeField] private Map map;
        [SerializeField] private List<Character> _characters;

        [Header("Events")] 
        [SerializeField] private UnityEvent characterSelected;

        [Header("Options")] 
        [SerializeField] private bool showDebugParameters;
        
        public event Action<Character> CharacterSelectedEvent;
        public event Action<Character> UpdateChatEvent;

        public CharactersConfig CharactersConfig => charactersConfig;
        public Character[] Characters => _characters.ToArray();

        public StoryResolver StoryResolver => storyResolver;
        public bool NeedPush { get; set; }
        public Character CurrentCharacter { get; set; }

        private СonversationData _lockedConversation;

        private СonversationData _currentConversation;
        private SwitcherInDialogue _switcher;

        private int _currentChatIndex;

        private void Awake()
        {
            //StartCoroutine(LoadCharacterSaves());
            RegisterCharacters();
        }

        private void Start()
        {
            Initialize();
            _saver.Initialize();
        }

        private void Initialize()
        {
            storyResolver.Initialize();
        }

        public void SaveCharacters() 
        {
            foreach (var character in _characters) 
            {
                _saver.mainData.AddCharacterData(character.Data);
            }

            _saver.SAVE();
        }

        private IEnumerator LoadCharacterSaves()
        {
            yield return _saver.LOAD();
            
            foreach (var character in _characters) 
            {
                _saver.mainData.LoadCharacterData(character.Data);
            }
        }

        public CharacterData GetCurrentCharacterData()
        {
            return CurrentCharacter.Data;
        }

        private void OnDestroy()
        {
            UnRegisterStartDialogue();
            UnRegisterCharacters();
            ResetCharacter();
        }

        public void SelectCurrentCharacter()
        {
            if (CurrentCharacter is null) return;
            
            CharacterSelectedEvent?.Invoke(CurrentCharacter);
            characterSelected?.Invoke();
            
            Debug.Log("Selected character: " + CurrentCharacter.name);
        }

        public void SelectCharacter(Character character)
        {
            CurrentCharacter = character;
            CharacterSelectedEvent?.Invoke(character);
            characterSelected?.Invoke();
            UpdateChatEvent?.Invoke(character);
            Debug.Log("Selected character: " + character.name);
        }

        public void InvokeGAEventCurrentCharacter(string eventName)
        {
            GameAnalytics.NewDesignEvent("girl_" + CurrentCharacter.Data.name + "_" + eventName);
        }

        public void RemoveCharacter(CharacterData characterData)
        {
            _characters.Remove(_characters.FirstOrDefault(x => x.Data == characterData));
        }

        public void OnCharacterSelected(Character character)
        {
            if (CurrentCharacter != null)
            {
                Debug.LogWarning("Current character: " + CurrentCharacter.name);
            }
            
            //if (chatSystem != null) chatSystem.gameObject.Activate();

            if (CurrentCharacter != character) _chat.ResetContent();

            CurrentCharacter = character;
            
            characterSelected?.Invoke();
            _eventManager.UpdateScrollChat();

            _currentConversation = CurrentCharacter.CurrentConversation;
            storyResolver.InitCharacterData(CurrentCharacter.Data);

            if (CurrentCharacter == null) 
	        	Debug.LogError("current character is null on selected");

            if (chatSystem != null) _chatInitializationModule.InstallCharacter(CurrentCharacter);
            
            SetLockedConversation(CurrentCharacter);

            CharacterSelectedEvent?.Invoke(character);
            UpdateChatEvent?.Invoke(character);

            map.CloseMap();
            
            previewerPanel.Activate();
            previewerPanel.SafeActivate();
        }

        private void UnRegisterStartDialogue()
        {
            if (CurrentCharacter == null)
            {
                Debug.LogWarning("Character is null, cannot unregister options");
                return;
            }
        }

        public void ReduceMoneyPlayer()
        {
            if (_bank.Money >= _chat.Config.CoinsForMessage)
            {
                _bank.ChangeValueGold(-_chat.Config.CoinsForMessage);
            }  
            else
            {
                _eventManager.ConversationEnded();
                Debug.Log("Money doesn't enough to continue...");
            }
        }       

        public void AddDiamondOnConversationEnd()
        {
            _bank.ChangeValueDiamonds(1);
        }

        public void AddLoyalty(int n)
        {
            CurrentCharacter.Data.ChangeLoyalty(n);
        }

        private void ResetCharacter()
        {
            CurrentCharacter = null;
            _currentConversation = null;
        }

        public void RegisterCharacter(Character character)
        {
            character.CharacterSelected += OnCharacterSelected;
        }

        public void UnregisterCharacter(Character character)
        {
            character.CharacterSelected -= OnCharacterSelected;
        }

        private void RegisterCharacters()
        {
            foreach (var character in _characters)
            {
                character.CharacterSelected += OnCharacterSelected;
                character.CharacterEnter += levelChecker.Enter;
                character.CharacterExit += levelChecker.Exit;
            }
        }

        public void SetLockedConversation(Character character)
        {
            CharacterData data = character.Data;
            List<СonversationData> conversations = data.allConversations;

            foreach (var conversation in conversations)
            { 
                //if (conversation.IsUnlocked(playerConfig.Experience)) _lockedConversation = null;
                
                if (conversation.isUnlocked == false)
                {
                    data.LockedConversation = conversation;
                    _lockedConversation = conversation;
                    
                    //_eventManager.SelectCharacter(character);
                    break;
                }
            }

            //Debug.Log("<color=red>Locked conversation is null!</color>");
        }

        private void UnRegisterCharacters()
        {
            foreach (var character in _characters)
            {
                character.CharacterSelected -= OnCharacterSelected;
                character.CharacterEnter -= levelChecker.Enter;
                character.CharacterExit -= levelChecker.Exit;
            }
        }
    }
}