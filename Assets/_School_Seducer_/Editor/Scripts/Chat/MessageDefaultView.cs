using System;
using System.Collections;
using System.Collections.Generic;
using _BonGirl_.Editor.Scripts;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class MessageDefaultView : MessageViewBase, IMessage
    {
        [SerializeField] private Button audioButtonLeftActor;
        [SerializeField] private Button audioButtonRightActor;

        private List<LocalizedScriptableObject.LocalizedData> _localizedData;

        public Button AudioButtonLeftActor => audioButtonLeftActor;
        public Button AudioButtonRightActor => audioButtonRightActor;    
            
        public MessageSender MessageSender { get; set; }

        private Transform _content;
        private SoundHandler _soundHandler;

        public void Initialize(OptionButton[] optionButtons)
        {
            MessageSender = Sender;
            OptionButtons = optionButtons;
        }

        public void InitContentSpace(Transform content)
        {
            _content = content;
        }

        public void InitSoundInvoker(SoundHandler soundHandler)
        {
            _soundHandler = soundHandler;
        }

        public void RenderGeneralData(MessageData data, Sprite actorLeft, Sprite actorRight, Sprite storyTeller, bool needIconStoryTeller)
        {
            SetSenderMsg(actorRight, actorLeft, storyTeller, data, needIconStoryTeller);
            SetParentActor();
            SetOptions(data);
            SetAudioButton();
        }

        public void TranslateText(string languageCode)
        {
            msgText.text = Data.TranslateMsg(languageCode);
        }

        public void SetNameActors(string leftActor, string rightActor, string storyTeller)
        {
            SetName(leftActor, rightActor, storyTeller);
        }

        private void SetParentActor()
        {
            AdjustRightActor(_content, Sender);
            AdjustStoryTeller(_content, Sender);
        }

        private void SetAudioButton()
        {
            if (Data.IsPictureMsg() == false)
            {
                if (Sender == MessageSender.ActorLeft)
                {
                    audioButtonLeftActor.onClick.AddListener(InvokeAudioMsg);
                    audioButtonRightActor.gameObject.Deactivate();
                }
                else if (Sender == MessageSender.ActorRight)
                {
                    audioButtonRightActor.onClick.AddListener(InvokeAudioMsg);
                    audioButtonRightActor.gameObject.Activate();
                    audioButtonLeftActor.gameObject.Deactivate();
                }
                else if (Sender == MessageSender.StoryTeller)
                {
                    audioButtonLeftActor.onClick.AddListener(InvokeAudioMsg);
                    audioButtonRightActor.gameObject.Deactivate();
                    RectTransform audioLeftRect = audioButtonLeftActor.GetComponent<RectTransform>();
                    audioLeftRect.Translate(Vector2.left * 6.2f);
                }

                _soundHandler.InvokeClipAfterExistClip(InvokeAudioMsg);
            }
        }

        private void InvokeAudioMsg()
        {
            //_soundHandler.AutoManagePlayback(Data.AudioMsg);
            
            if (_soundHandler.IsClipPlaying())
                _soundHandler.StopClip();
            else
                _soundHandler.InvokeClipAfterPlayback(() =>_soundHandler.InvokeOneClip(Data.AudioMsg));
        }

        private void OnDestroy()
        {
            if (Data.AudioMsg != null)
            {
                audioButtonLeftActor.RemoveListener(InvokeAudioMsg);
                audioButtonRightActor.RemoveListener(InvokeAudioMsg);
            }
        }
    }
}