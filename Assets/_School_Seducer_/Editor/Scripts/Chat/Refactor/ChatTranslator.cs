using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using TMPro;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat.Refactor
{
    public class ChatTranslator : MonoBehaviour, IChatTranslationModule
    {
        [Inject] private LocalizedGlobalMonoBehaviour _localizer;

        private ChatSystem _chatSystem;

        private List<MessageDefaultViewProxy> _defaultMessages;
        private List<MessageData> _messagesData;
        
        public void InitializeCore(ChatSystem system)
        {
            _chatSystem = system;
        }

        private void Start()
        {
            _localizer.AddObserver(this);
        }

        private void OnDestroy()
        {
            _localizer.RemoveObserver(this);
        }
        
        public void UpdateMessages(List<MessageData> messages)
        {
            _messagesData = messages;
        }

        public void OnObservableUpdate()
        {
            foreach (var defaultMessage in _defaultMessages)
            {
                defaultMessage.MsgText.Text = TranslateTextMessage(defaultMessage.Data);
            }
            
            foreach (var msg in _messagesData)
            {
                msg.Msg = msg.TranslateTextMsg(_localizer.GlobalLanguageCodeRuntime);
                var translatedMsg = msg.TranslateAudioMsg(_localizer.GlobalLanguageCodeRuntime);
                msg.AudioMsg = translatedMsg.translatedAudio;
            }
            
            //var paddingBack = CreatePadding();
            //paddingBack.gameObject.Deactivate(0.1f);
        }

        public void TranslateDefaultMessages(List<MessageDefaultViewProxy> messages)
        {
            _defaultMessages = messages;
            
            foreach (var msg in _defaultMessages)
            {
                msg.MsgText.Text = TranslateTextMessage(msg.Data);
            }
        }

        public void TranslateAudioClips()
        {
            
        }

        private string TranslateTextMessage(MessageData msgData)
        {
            return msgData.TranslateTextMsg(_localizer.GlobalLanguageCodeRuntime);
        }
    }
}