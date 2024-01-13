using System;
using _School_Seducer_.Editor.Scripts.Chat;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityFigmaBridge.Runtime.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ChatStatusView : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private TextMeshProUGUI chatPercentCompletion;
        [SerializeField] private TextMeshProUGUI chatName;
        [SerializeField] private FigmaImage chatBarImage;
        [SerializeField] private FigmaImage chatStatusImage;
        
        public СonversationData Conversation { get; private set; }
        private Chat.Chat _chat;

        public event Action<int, MessageData[]> OnCompletionChanged;
        public event Action OnClick;

        public void Initialize(Chat.Chat chat)
        {
            _chat = chat;
        }

        private void Start()
        {
            OnCompletionChanged += SetPercentCompletion;
        }

        private void OnDestroy()
        {
            OnCompletionChanged -= SetPercentCompletion;
        }

        public void Render(СonversationData chatData)
        {
            Conversation = chatData;
            chatName.text = chatData.name;
        }

        public void SetStatus(Sprite completed, Sprite uncompleted)
        {
            chatStatusImage.sprite = Conversation.isCompleted ? completed : uncompleted;
        }

        public void CompletionChanged(int messageIndex, MessageData[] messages)
        {
            OnCompletionChanged?.Invoke(messageIndex, messages);
        }

        private void SetPercentCompletion(int currentMessageIndex, MessageData[] messages)
        {
            float currentPercents = (float)(currentMessageIndex + 1) / messages.Length * 100f;
            chatPercentCompletion.text = Mathf.RoundToInt(currentPercents) + "%";
            chatBarImage.fillAmount = currentPercents / 100f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _chat.InstallCurrentStatusView(this);
            OnClick?.Invoke();
        }
    }
}