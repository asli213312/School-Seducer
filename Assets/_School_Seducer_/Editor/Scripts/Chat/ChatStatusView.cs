using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityFigmaBridge.Runtime.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ChatStatusView : LocalizedMonoBehaviour, IPointerDownHandler
    {
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;

        [SerializeField] private RectTransform pointUnlocked;
        [SerializeField] private TextMeshProUGUI storyLabel;
        [SerializeField] private Slider barToUnlock;
        [SerializeField] private FigmaImage storyIcon;

        public RectTransform PointUnlocked => pointUnlocked;
        public СonversationData Conversation { get; private set; }

        private Chat.Chat _chat;
        private Sprite _uncompletedSprite;
        private Sprite _bgStartSprite;
        private Image _bg;

        public event Action OnClick;
        public event Action<ChatStatusView> OnEnter;
        public event Action<ChatStatusView> SelectEvent;

        public void Initialize(Chat.Chat chat, LocalizedGlobalMonoBehaviour localizer)
        {
            _chat = chat;
            _localizer ??= localizer;
        }

        private void Start()
        {
            _bg = transform.GetChild(0).GetComponent<Image>();
            _bgStartSprite = _bg.sprite;

            //transform.Rotate(new Vector3(0, 0, 180));
        }

        public void OnUpdateUnlockBar(float newValueBar)
        {
            if (Conversation.isUnlocked == false)
            {
                barToUnlock.value = newValueBar;
            }
            else
            {
                HideBarUnlock();
            }
            
            SetStatus();  
        }

        public void HideBarUnlock()
        {
            barToUnlock.gameObject.Deactivate();
            SetStatus();
        }

        public void Render(СonversationData chatData, Sprite uncompletedSprite)
        {
            Conversation = chatData;

            InstallLocalizedData(chatData);
            ProvideLocalizer(_localizer);

            storyLabel.text = UpdateLocalizedDataById($"{chatData.name}");
            Host = storyLabel;

            storyIcon.sprite = chatData.iconStory;

            _uncompletedSprite = uncompletedSprite;
            
            barToUnlock.maxValue = Conversation.costExp;
            
            if (Conversation.isSeen == false && Conversation.isUnlocked)
                pointUnlocked.gameObject.Activate();

            SetStatus();
        }

        public void ActivateSelected() => _bg.sprite = _chat.Config.selectedStatusView;

        public void ResetSelected() => _bg.sprite = _bgStartSprite;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Conversation.isSeen == false)
            {
                Conversation.isSeen = true;
                pointUnlocked.gameObject.Deactivate();
            }
            
            OnEnter?.Invoke(this);
            
            SelectEvent?.Invoke(this);
            _chat?.InstallStatusView(this);
            OnClick?.Invoke();
        }

        private void SetStatus()
        {
            storyIcon.sprite = Conversation.isUnlocked ? Conversation.iconStory : _uncompletedSprite;

            if (_chat != null && _chat.CurrentCharacterData != null)
            {
                if (Conversation == _chat.CurrentCharacterData.LockedConversation)
                {
                    storyIcon.sprite = Conversation.iconStory;
                }
                else if (Conversation.isUnlocked)
                    storyIcon.sprite = Conversation.iconStory;
                else
                    storyIcon.sprite = _uncompletedSprite;
            }
        }

        // public void CompletionChanged(int messageIndex, MessageData[] messages)
        // {
        //     OnCompletionChanged?.Invoke(messageIndex, messages);
        // }

        // private void SetPercentCompletion(int currentMessageIndex, MessageData[] messages)
        // {
        //     float currentPercents = (float)(currentMessageIndex + 1) / messages.Length * 100f;
        //     chatPercentCompletion.text = Mathf.RoundToInt(currentPercents) + "%";
        //     barToUnlock.fillAmount = currentPercents / 100f;
        // }
    }
}