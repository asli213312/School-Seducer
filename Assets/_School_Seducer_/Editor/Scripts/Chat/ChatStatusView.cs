using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityFigmaBridge.Runtime.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ChatStatusView : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TextMeshProUGUI storyLabel;
        [SerializeField] private Slider barToUnlock;
        [SerializeField] private FigmaImage storyIcon;
        
        public СonversationData Conversation { get; private set; }
        private Chat.Chat _chat;
        private Sprite _uncompletedSprite;
        
        public event Action OnClick;

        public void Initialize(Chat.Chat chat)
        {
            _chat = chat;
        }

        private void Start()
        {
            transform.Rotate(new Vector3(0, 0, 180));
        }

        public void OnUpdateUnlockBar()
        {
            if (StoryUnlocked() == false)
            {
                barToUnlock.value = _chat.CurrentCharacter.experience;
                SetStatus();    
            }
            else
                HideBarUnlock();
        }

        public void HideBarUnlock()
        {
            barToUnlock.gameObject.Deactivate();
        }

        public void Render(СonversationData chatData, Sprite uncompletedSprite)
        {
            Conversation = chatData;
            storyLabel.text = chatData.name;
            storyIcon.sprite = chatData.iconStory;

            _uncompletedSprite = uncompletedSprite;
            
            barToUnlock.maxValue = Conversation.costExp;
            
            OnUpdateUnlockBar();
        }

        private void SetStatus()
        {
            storyIcon.sprite = Conversation.isUnlocked ? Conversation.iconStory : _uncompletedSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (StoryUnlocked() == false) return;
            
            _chat.InstallCurrentStatusView(this);
            OnClick?.Invoke();
        }

        private bool StoryUnlocked() => Conversation.isUnlocked;

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