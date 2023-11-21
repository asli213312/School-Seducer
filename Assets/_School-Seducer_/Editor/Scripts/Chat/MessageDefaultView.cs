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
        public void Initialize(OptionButton[] optionButtons)
        {
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

        public bool CheckBigMessage()
        {
            RectTransform msgRect = msgText.GetComponent<RectTransform>();
            RectTransform backMsgRect = backMsg.GetComponent<RectTransform>();
            if (msgText.preferredHeight > backMsg.GetComponent<RectTransform>().sizeDelta.y)
            {
                float heightDifference = msgText.preferredHeight - backMsgRect.sizeDelta.y;

                // Увеличиваем высоту backMsg
                backMsgRect.sizeDelta = new Vector2(backMsgRect.sizeDelta.x,
                    backMsgRect.sizeDelta.y + heightDifference / 1.1f);

                //Vector3 initActorSize = actorRect.localScale;
                //Vector2 initBaseSize = baseRect.sizeDelta;
                //baseRect.sizeDelta = new Vector2(baseRect.sizeDelta.x, baseRect.sizeDelta.y + heightDifference / 1.3f);
                msgRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, backMsgRect.sizeDelta.y);
                return true;
            }

            return false;
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