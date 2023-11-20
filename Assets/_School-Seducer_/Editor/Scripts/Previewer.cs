using System;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using DialogueEditor;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UltEvents;
using UnityEngine.Events;
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
        [SerializeField] private Image _emote;
        [SerializeField] private TextMeshProUGUI greetingsText;
        [SerializeField] private Button startDialogueButton;
        [SerializeField] private Button addMoneyByMiniGameButton;
        [SerializeField] private Button closePreviewerPanel;

	    [Header("Data")] 
	    [SerializeField] private Chat.Chat _chat;
        [SerializeField] private LevelChecker levelChecker;
        [SerializeField] private MiniGameInitializer miniGameInitializer;
        [SerializeField] private Map map;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Character[] _characters;

        [Header("Options")] 
        [SerializeField] private bool showDebugParameters;

        private Character _currentCharacter;
        private СonversationData _currentConversation;
        private SwitcherInDialogue _switcher;

        private int _selectedNodeID;

        private Sprite _initialSprite;
        private Sprite _initialEmote;

        public void Initialize(Chat.Chat chat)
        {
            foreach (var character in _characters)
            {
                character.Initialize(chat);
            }
        }

        private void Awake()
        {
            _switcher = GetComponent<SwitcherInDialogue>();
            
            addMoneyByMiniGameButton.AddListener(miniGameInitializer.StartMiniGame);
            closePreviewerPanel.AddListener(previewerPanel.Deactivate);
            
            RegisterCharacters();

            _initialSprite = selectedGirlImage.sprite;
            _initialEmote = _emote.sprite;
        }

        private void OnEnable()
        {
            SetInitialSpriteAndEmote();
        }

        private void OnDestroy()
        {
            addMoneyByMiniGameButton.RemoveListener(miniGameInitializer.StartMiniGame);

            closePreviewerPanel.RemoveListener(previewerPanel.Deactivate);
            startDialogueButton.RemoveAllListeners();

            UnRegisterStartDialogue();
            UnRegisterConversation();
            UnRegisterCharacters();
            ResetCharacter();

            _switcher.Reset();
        }

        private void OnCharacterSelected(Character character)
        {
            previewerPanel.Activate();

            _currentCharacter = character;
	        _currentConversation = _currentCharacter.Conversation;
            
	        if (_currentCharacter == null) 
	        	Debug.LogError("current character is null on selected");

            Invoke(nameof(RegisterConversation), 0.2f);
            Invoke(nameof(RegisterStartDialogue), 0.3f);
            if (!CheckLevelPlayer())
            {
                Debug.Log("Check for level is invoked");
                startDialogueButton.AddListener(SkipDialog);
            }

            _switcher.Initialize(_emote, selectedGirlImage);
            //Invoke(nameof(SetCurrentNodeID), 3f);

	        map.CloseMap();
	        _chat.InstallCurrentConversation(_currentConversation);

            //UpdateUI();
        }

        public void SkipDialog()
        {
            
        }

        private bool CheckLevelPlayer()
        {
            return playerConfig.Level >= _currentCharacter.Data.RequiredLevel;
        }

        private void ShadowOutPreviewer()
        {
            if (previewerPanel.activeSelf == false) return;
            
            if (_emote.gameObject.activeSelf && _emote != null) _emote.gameObject.Deactivate();
            
            previewerPanel.Deactivate();
        }

        private void SetInitialSpriteAndEmote()
        {
            selectedGirlImage.sprite = _initialSprite;
            _emote.sprite = _initialEmote;
        }

        private void RegisterConversation()
        {
            //ConversationManager.OnButtonClicked += UpdateUI;
        }

        private void UnRegisterConversation()
        {
            //ConversationManager.OnButtonClicked -= UpdateUI;
        }

        private void RegisterStartDialogue()
        {
            if (_currentCharacter == null)
            {
                Debug.LogWarning("Character is null, cannot register options");
                //return;
            }
            
            startDialogueButton.AddListener(_currentCharacter.StartConversation);
        }

        private void UnRegisterStartDialogue()
        {
            if (_currentCharacter == null)
            {
                Debug.LogWarning("Character is null, cannot unregister options");
                return;
            }
            
            startDialogueButton.RemoveListener(_currentCharacter.StartConversation);
        }

        public void CheckMoneyPlayer()
        {
            if (_bank.Money >= 1)
            {
                _bank.ChangeValueMoney(-playerConfig.CostNextNode);
            }  
            else
            {
                _currentCharacter.EndConversation();
            }
        }       

        public void AddDiamondOnConversationEnd()
        {
            _bank.ChangeValueDiamonds(1);
        }

        private void UpdateUI()
        {
            greetingsText.text = "Hello, " + _currentCharacter.name;
            selectedGirlImage.sprite = _currentCharacter.SpriteRenderer.sprite;
        }

        public void AddLoyalty(int n)
        {
            _currentCharacter.Data.ChangeLoyalty(n);
        }

        private void ResetCharacter()
        {
            _currentCharacter = null;
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