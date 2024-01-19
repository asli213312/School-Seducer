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
        [SerializeField] private Image background;
        [SerializeField] private Button audioButtonLeftActor;
        [SerializeField] private Button audioButtonRightActor;

        private List<LocalizedScriptableObject.LocalizedData> _localizedData;
        
        private Button _currentAudioButtonActor;

        public MessageSender MessageSender => Sender;

        private Transform _content;
        private SoundHandler _soundHandler;
        private string _currentLanguageCode;

        public void Initialize(OptionButton[] optionButtons)
        {
            OptionButtons = optionButtons;
        }

        public bool NeedPaddings() => Sender != MessageSender.StoryTeller && Sender != MessageSender.ActorRight;

        public void SetBlockBackground(Sprite leftActor, Sprite rightActor, Sprite storyTeller)
        {
            Sprite selectedBlock = null;
            
            switch (Sender)
            {
                case MessageSender.ActorLeft: selectedBlock = leftActor; break;
                case MessageSender.ActorRight: selectedBlock = rightActor; break;
                case MessageSender.StoryTeller: selectedBlock = storyTeller; break;
            }

            background.sprite = selectedBlock;
        }

        public void SetFontColor(Color actorLeft, Color actorRight, Color storyTeller)
        {
            Color selectedColor = Color.clear;
            
            switch (Sender)
            {
                case MessageSender.ActorLeft: selectedColor = actorLeft; break;
                case MessageSender.ActorRight: selectedColor = actorRight; break;
                case MessageSender.StoryTeller: selectedColor = storyTeller; break;
            }
            
            msgText.color = selectedColor;
        }

        public void InstallLanguageCode(string languageCode) => _currentLanguageCode = languageCode;

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
            SetAudioButton();
            SetParentActor();
            SetOptions(data);
        }

        public void TranslateText(string languageCode)
        {
            msgText.text = Data.TranslateTextMsg(languageCode);
        }

        public void TranslateAudio(string languageCode)
        {
            var (isTranslated, translatedAudioMsg) = Data.TranslateAudioMsg(languageCode);

            if (isTranslated)
            {
                if (_currentAudioButtonActor != null)
                    _currentAudioButtonActor.gameObject.Activate();
                else
                    Debug.LogWarning("CurrentAudioButtonActor is null to change status view");
                
                Debug.Log("AudioMessage was translated: " + translatedAudioMsg.name);
            }
            else
            {
                if (_currentAudioButtonActor != null)
                    _currentAudioButtonActor.gameObject.Deactivate();
                
                if (translatedAudioMsg != null)
                    Debug.Log("AudioMessage wasn't translated: " + translatedAudioMsg.name);
            }

            _currentLanguageCode = languageCode;
            //Data.AudioMsg = translatedAudioMsg;
        }

        public void SetNameActors(string leftActor, string rightActor, string storyTeller)
        {
            SetName(leftActor, rightActor, storyTeller);
        }

        public void InstallStatusTranslatedAudioButtons()
        {
            var translatedAudio = Data.TranslateAudioMsg(_currentLanguageCode);

            if (translatedAudio.isTranslated)
            {
                if (Sender == MessageSender.ActorLeft)
                {
                    audioButtonLeftActor.gameObject.Activate();
                    audioButtonRightActor.gameObject.Deactivate();
                }
                else if (Sender == MessageSender.ActorRight)
                {
                    audioButtonRightActor.gameObject.Activate();
                    audioButtonLeftActor.gameObject.Deactivate();
                }
                else if (Sender == MessageSender.StoryTeller)
                {
                    audioButtonLeftActor.gameObject.Activate();
                    audioButtonRightActor.gameObject.Deactivate();
                }
            }
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
                    _currentAudioButtonActor = audioButtonLeftActor;
                }
                else if (Sender == MessageSender.ActorRight)
                {
                    audioButtonRightActor.onClick.AddListener(InvokeAudioMsg);
                    //audioButtonRightActor.gameObject.Activate();
                    audioButtonLeftActor.gameObject.Deactivate();
                    _currentAudioButtonActor = audioButtonRightActor;
                }
                else if (Sender == MessageSender.StoryTeller)
                {
                    audioButtonLeftActor.onClick.AddListener(InvokeAudioMsg);
                    audioButtonRightActor.gameObject.Deactivate();
                    _currentAudioButtonActor = audioButtonLeftActor;

                    RectTransform audioLeftRect = audioButtonLeftActor.GetComponent<RectTransform>();
                    audioLeftRect.anchorMax = new Vector2(0, 0.5f);
                    audioLeftRect.anchorMin = new Vector2(0, 0.5f);
                    audioLeftRect.Translate(Vector2.left * 1.3f);
                }

                _soundHandler.InvokeClipAfterExistClip(InvokeAudioMsg);
            }
        }

        private void InvokeAudioMsg()
        {
            //_soundHandler.AutoManagePlayback(Data.AudioMsg);

            var (isTranslated, translatedAudioMsg) = Data.TranslateAudioMsg(_currentLanguageCode);

            if (isTranslated)
            {
                if (_currentAudioButtonActor != null)
                    _currentAudioButtonActor.gameObject.Activate();
                else
                    Debug.LogWarning("CurrentAudioButtonActor is null to change status view");
                
                if (_soundHandler.IsClipPlaying())
                    _soundHandler.StopClip();
                else
                    _soundHandler.InvokeClipAfterPlayback(() =>_soundHandler.InvokeOneClip(Data.AudioMsg));
            }
            else
                _currentAudioButtonActor.gameObject.Deactivate();
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