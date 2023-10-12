using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using DialogueEditor;
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
        [SerializeField] private Text moneyText;
        [SerializeField] private Image selectedGirlImage;
        [SerializeField] private Image _emote;
        [SerializeField] private TextMeshProUGUI greetingsText;
        [SerializeField] private Button startDialogueButton;
        [SerializeField] private Button addMoneyByMiniGameButton;
        [SerializeField] private Button closePreviewerPanel;

        [Header("Data")] 
        [SerializeField] private LevelChecker levelChecker;
        [SerializeField] private MiniGameInitializer miniGameInitializer;
        [SerializeField] private Map map;
        [SerializeField] private PlayerConfig playerConfig;
        [SerializeField] private Character[] _characters;

        private Character _currentCharacter;
        private NPCConversation _currentConversation;
        private SwitcherInDialogue _switcher;

        private Sprite _initialSprite;
        private Sprite _initialEmote;

        private void Awake()
        {
            _switcher = GetComponent<SwitcherInDialogue>();
            
            _eventManager.ChangeValueMoneyEvent += UpdateMoneyText;
            _eventManager.UpdateTextMoneyEvent += UpdateUI;
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

        private void OnDisable()
        {
            startDialogueButton.onClick.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            _eventManager.ChangeValueMoneyEvent -= UpdateMoneyText;
            _eventManager.UpdateTextMoneyEvent -= UpdateUI;
            addMoneyByMiniGameButton.RemoveListener(miniGameInitializer.StartMiniGame);
            
            closePreviewerPanel.RemoveListener(previewerPanel.Deactivate);
            startDialogueButton.RemoveAllListeners();

            UnRegisterConversation();
            UnRegisterCharacters();

            _switcher.Reset();
        }

        private void OnCharacterSelected(Character character)
        {
            previewerPanel.Activate();
            
            _currentCharacter = character;
            _currentConversation = _currentCharacter.Dialogue;
            RegisterStartDialogue();
            RegisterConversation();
            if (!CheckLevelPlayer())
            {
                Debug.Log("Check for level is invoked");
                startDialogueButton.AddListener(SkipDialog);
            }

            _switcher.Initialize(_emote, selectedGirlImage);

            map.CloseMap();

            UpdateUI();
        }

        public void SkipDialog()
        {
            if (ConversationManager.Instance.IsConversationActive)
            {
                ConversationManager.Instance.SetBool("skip", true);
                Debug.Log("Skip: " + ConversationManager.Instance.GetBool("skip"));    
            }
        }

        private bool CheckLevelPlayer()
        {
            return playerConfig.Level >= _currentCharacter.Data.RequiredLevel;
        }

        private void ShadowOutPreviewer()
        {
            if (_emote.gameObject.activeSelf && _emote != null) _emote.gameObject.Deactivate();
            
            previewerPanel.Deactivate();
        }

        private void StartDialogueDueMoney()
        {
            if (_bank.Money >= playerConfig.CostChooseOption)
                _eventManager.ChangeValueMoney(-playerConfig.CostChooseOption);
            else
            {
                //Invoke("ShadowOutPreviewer", 5f);
                Debug.Log("Not enough money!");
            }
        }

        private void SetInitialSpriteAndEmote()
        {
            selectedGirlImage.sprite = _initialSprite;
            _emote.sprite = _initialEmote;
        }

        private void RegisterStartDialogue()
        {
            if (_currentCharacter == null)
            {
                Debug.LogWarning("Character is null, cannot register options");
                return;
            }
            
            startDialogueButton.AddListener(_currentCharacter.StartConversation);
            startDialogueButton.AddListener(StartDialogueDueMoney);
        }

        private void RegisterConversation()
        {
            ConversationManager.OnConversationStarted += RegisterStartDialogue;
            ConversationManager.OnConversationEnded += UnRegisterStartDialogue;
            ConversationManager.OnConversationEnded += ShadowOutPreviewer;
        }

        private void UnRegisterConversation()
        {
            ConversationManager.OnConversationStarted -= RegisterStartDialogue;
            ConversationManager.OnConversationEnded -= UnRegisterStartDialogue;
            ConversationManager.OnConversationEnded -= ShadowOutPreviewer; 
        }

        private void UnRegisterStartDialogue()
        {
            if (_currentCharacter == null)
            {
                Debug.LogWarning("Character is null, cannot unregister options");
                return;
            }
            
            startDialogueButton.RemoveListener(_currentCharacter.StartConversation);
            startDialogueButton.RemoveListener(StartDialogueDueMoney);
        }

        private void UpdateUI()
        {
            greetingsText.text = "Hello, " + _currentCharacter.name;
            moneyText.text = "Money: " + _bank.Money;
            selectedGirlImage.sprite = _currentCharacter.SpriteRenderer.sprite;
        }
        
        private void UpdateMoneyText(int moneyValue)
        {
            moneyText.text = "Money: " + moneyValue;
        }

        private void ResetCharacter()
        {
            _currentCharacter = null;
            _currentConversation = null;
            
            previewerPanel.Deactivate();
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