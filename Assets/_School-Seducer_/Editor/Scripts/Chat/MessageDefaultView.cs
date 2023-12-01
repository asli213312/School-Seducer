using System;
using _BonGirl_.Editor.Scripts;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class MessageDefaultView : MessageViewBase, IMessage
    {
        [SerializeField] private Button audioButton;
        
        public MessageSender MessageSender { get; set; }

        private SoundInvoker _soundInvoker;

        public void Initialize(OptionButton[] optionButtons)
        {
            MessageSender = Sender;
            OptionButtons = optionButtons;
        }

        public void InitSoundInvoker(SoundInvoker soundInvoker)
        {
            _soundInvoker = soundInvoker;
        }

        public void RenderGeneralData(MessageData data, Sprite actorLeft, Sprite actorRight, Sprite storyTeller, bool needIconStoryTeller)
        {
            SetSenderMsg(actorRight, actorLeft, storyTeller, data, needIconStoryTeller);
            SetOptions(data);
            SetAudioButton();
            Debug.Log("Options were installed");
        }

        public void SetNameActors(string leftActor, string rightActor, string storyTeller)
        {
            SetName(leftActor, rightActor, storyTeller);
        }

        public bool CheckIsVeryBigMessage()
        {
            Debug.Log("IsBigMessage: " + IsBigMessage);
            Debug.Log("IsVeryBigMessage: " + IsVeryBigMessage);
            Debug.Log("Message length: " + msgText.text.Length);

            return IsVeryBigMessage;
        }

        public bool IsBigMessageFalse()
        {
            return IsBigMessage == false;
        }

        private void SetAudioButton()
        {
            if (Data.IsPictureMsg() == false)
            {
                audioButton.onClick.AddListener(InvokeAudioMsg);
            }
        }

        private void InvokeAudioMsg()
        {
            _soundInvoker.InvokeClip(Data.AudioMsg, audioButton.gameObject.Deactivate, 5f);
        }

        private void SetOptions(MessageData data)
        {
            for (int i = 0; i < OptionButtons.Length && i < data.optionalData.Branches.Length; i++)
            {
                if (data.optionalData.Branches[i] == null) break;

                if (data.optionalData.Branches[i] != null)
                {
                    OptionButtons[i].BranchData = data.optionalData.Branches[i];
                    Text textChildren = OptionButtons[i].GetComponentInChildren<Text>();
                    textChildren.text = OptionButtons[i].BranchData.BranchName;
                }
            }
        }

        private void OnDestroy()
        {
            if (Data.AudioMsg != null) audioButton.RemoveListener(InvokeAudioMsg);
        }
    }
}