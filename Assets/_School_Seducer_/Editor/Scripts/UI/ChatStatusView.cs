using System;
using _School_Seducer_.Editor.Scripts.Chat;
using TMPro;
using UnityEngine;
using UnityFigmaBridge.Runtime.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class ChatStatusView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI chatPercentCompletion;
        [SerializeField] private TextMeshProUGUI chatName;
        [SerializeField] private FigmaImage chatBarImage;
        [SerializeField] private FigmaImage chatStatusImage;

        private СonversationData _chatData;

        public event Action<int, MessageData[]> OnCompletionChanged;

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
            _chatData = chatData;
            chatName.text = chatData.name;
        }

        public void SetStatus(Sprite completed, Sprite uncompleted)
        {
            chatStatusImage.sprite = _chatData.IsCompleted ? completed : uncompleted;
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
    }
}