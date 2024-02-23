using System;
using System.Collections.Generic;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class ChatStateHandler : MonoBehaviour, IChatStateHandlerModule
    {
        public bool IsMessagesEnded { get; set; }
        
        private ChatSystem _chatSystem;
        private IChatMessageProcessorModule _messageProcessor;

        public void InitializeCore(ChatSystem chatSystem)
        {
            _chatSystem = chatSystem;
            _messageProcessor = _chatSystem.MessageProcessor;
        }
        
        public void StartConversation(Action entryAction = null)
        {
            entryAction?.Invoke();
            InvokeStartConversation();
        }

        public void InvokeStartConversation()
        {
            _messageProcessor.StartProcessMessages();
        }

        public void LoadMessages(MessageData[] messages = null)
        {
            _messageProcessor.StartProcessMessages(messages);
        }
        
        public void TryEndConversation(bool isLastMessage = false)
        {
            if (isLastMessage)
            {
                _chatSystem.ChatConfig.OnConversationEnd?.Invoke();
                IsMessagesEnded = true;
            }
        }

        public void EndConversation()
        {
            _chatSystem.ChatConfig.OnConversationEnd?.Invoke();
            IsMessagesEnded = true;
        }

        public void ResetEndConversation()
        {
            IsMessagesEnded = false;
        }

        public bool CheckEndConversation() => IsMessagesEnded;

        public void ResetContent()
        {
            //throw new System.NotImplementedException();
        }
    }
}