using System.Linq;
using System.Runtime.CompilerServices;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using NaughtyAttributes;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class MessageDefaultView : MessageViewBase, IMessage
    {
        public MessageSender MessageSender { get; set; }

        public void Initialize(OptionButton[] optionButtons)
        {
            MessageSender = Sender;
            OptionButtons = optionButtons;
        }

        public void RenderGeneralData(MessageData data, Sprite actorLeft, Sprite actorRight, Sprite storyTeller, bool needIconStoryTeller)
        {
            SetSenderMsg(actorRight, actorLeft, storyTeller, data, needIconStoryTeller);
            SetOptions(data);
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
    }
}