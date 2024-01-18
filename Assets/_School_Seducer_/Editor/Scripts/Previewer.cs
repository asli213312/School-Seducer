using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;

namespace _School_Seducer_.Editor.Scripts
{
    public class Previewer : MonoBehaviour
    {
        [Inject] private EventManager _eventManager;
        [Inject] private Bank _bank;
        
        [Header("UI Data")] 
        [SerializeField] private GameObject previewerPanel;
        [SerializeField] private Image selectedGirlImage;
        [SerializeField] private TextMeshProUGUI greetingsText;

        [Header("Data")] 
	    [SerializeField] private Chat.Chat _chat;
        [SerializeField] private StoryResolver storyResolver;
        [SerializeField] private LevelChecker levelChecker;
        [SerializeField] private MiniGameInitializer miniGameInitializer;
        [SerializeField] private Map map;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Character[] _characters;

        [Header("Options")] 
        [SerializeField] private bool showDebugParameters;

        public Character[] Characters => _characters;
        public StoryResolver StoryResolver => storyResolver;
        public bool NeedPush { get; set; }
        public Character CurrentCharacter { get; set; }
        
        private СonversationData _lockedConversation;

        private СonversationData _currentConversation;
        private SwitcherInDialogue _switcher;

        private int _currentChatIndex;

        private void Start()
        {
            Initialize();
        }

        //public void ShowChat() => _chat.gameObject.Activate();

        public void Initialize()
        {
            foreach (var character in _characters)
            {
                character.Initialize(_chat);
            }
            
            storyResolver.Initialize();
        }

        public bool FindLockedConversation(Sprite rolledLeftActorConversationSprite)
        {
            bool notFound = false;
            
            foreach (var character in _characters)
            {
                SetLockedConversation(character);
                
                if (character.Data.LockedConversation.ActorLeftSprite != rolledLeftActorConversationSprite) continue;

                Debug.Log("Character for locked conversation: " + character.Data.name);

                foreach (var conversation in character.Data.allConversations)
                {
                    if (conversation != character.Data.LockedConversation)  continue;
                    
                    if (conversation.ActorLeftSprite == rolledLeftActorConversationSprite)
                    {
                        //_lockedConversation = conversation;

                        if (conversation.isUnlocked) return false;
                        
                        storyResolver.SetRolledConversation(conversation);

                        storyResolver.UpdateStatusViews();
                        
                        _eventManager.UpdateTextExperience();

                        Debug.Log("Found rolled conversation: " + conversation.name);
                        return true;
                    }
                }
            }

            Debug.Log("Locked Conversation is null");
            return false;
        }

        public CharacterData GetCurrentCharacterData()
        {
            return CurrentCharacter.Data;
        }

        private void Awake()
        {
            RegisterCharacters();
        }

        private void OnDestroy()
        {
            UnRegisterStartDialogue();
            UnRegisterCharacters();
            ResetCharacter();
        }

        public void OnCharacterSelected(Character character)
        {
            if (CurrentCharacter != null)
            {
                Debug.LogWarning("Current character: " + CurrentCharacter.name);
            }
            
            previewerPanel.Activate();

            CurrentCharacter = character;
	        _currentConversation = CurrentCharacter.currentConversation;

            if (CurrentCharacter == null) 
	        	Debug.LogError("current character is null on selected");

            _chat.DeactivateStatusViews();
            _chat.ResetStatusViews();
            _chat.InstallCharacterData(CurrentCharacter.Data);
            
            //Invoke(nameof(RegisterStartDialogue), 0.3f);
            if (!CheckLevelPlayer())
            {
                Debug.Log("Check for level is invoked");
                //startDialogueButton.AddListener(SkipDialog);
            }

            //_switcher.Initialize(_emote, selectedGirlImage);
            //Invoke(nameof(SetCurrentNodeID), 3f);

	        map.CloseMap();
            //_chat.InstallCurrentConversation(_currentConversation);

            //UpdateUI();
        }

        public void SkipDialog()
        {
            
        }

        private bool CheckLevelPlayer()
        {
            return playerConfig.Level >= CurrentCharacter.Data.RequiredLevel;
        }

        private void RegisterStartDialogue()
        {
            if (CurrentCharacter == null)
            {
                Debug.LogWarning("Character is null, cannot register options");
                //return;
            }
            
            //_chat.StartDialogue(_currentCharacter.StartConversation);
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
                _bank.ChangeValueMoney(-_chat.Config.CoinsForMessage);
            }  
            else
            {
                CurrentCharacter.EndConversation();
                Debug.Log("Money doesn't enough to continue...");
            }
        }       

        public void AddDiamondOnConversationEnd()
        {
            _bank.ChangeValueDiamonds(1);
        }

        private void UpdateUI()
        {
            greetingsText.text = "Hello, " + CurrentCharacter.name;
            selectedGirlImage.sprite = CurrentCharacter.SpriteRenderer.sprite;
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
                    
                    _eventManager.SelectCharacter(character);
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