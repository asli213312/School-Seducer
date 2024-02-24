using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public abstract class MessageViewBase : MonoBehaviour
    {
        [SerializeField] protected Text msgNameText;
        [SerializeField] protected TextMeshProUGUI msgText;
        [SerializeField] protected Image leftBorderActor;
        [SerializeField] protected Image rightBorderActor;

        public MessageData Data;
        protected OptionButton[] OptionButtons;
        protected MessageSender Sender;

        private float _maxWidth;

        protected void SetSenderMsg(Sprite actorRight, Sprite actorLeft, Sprite storyTeller, MessageData data, bool needIconStoryTeller)
        {
            Data = data;
            Sender = data.Sender;

            switch (data.Sender)
            {
            case MessageSender.ActorRight:
                    //msgNameText.alignment = TextAnchor.UpperRight;
                    msgText.text = data.Msg;

                    rightBorderActor.gameObject.Activate();
                    Image rightIcon = rightBorderActor.transform.GetChild(0).GetComponent<Image>();
                    rightIcon.sprite = actorRight;
                    leftBorderActor.gameObject.Deactivate();
                    Debug.Log("Right actor installed");
                    break;
                
                case MessageSender.ActorLeft:
	                msgText.text = data.Msg;
                    Image leftIcon = leftBorderActor.transform.GetChild(0).GetComponent<Image>();
                    leftIcon.sprite = actorLeft;
                    Debug.Log("Base actor installed");
                    break;
                
                case MessageSender.StoryTeller:
                    //msgNameText.alignment = TextAnchor.UpperCenter;
                    msgText.text = data.Msg;

                    RectTransform mainRect = gameObject.GetComponent<RectTransform>();

                    //backMsg.Translate(Vector2.right * (0.15f), Space.Self);

                    Vector2 newSize; 
                    if (data.optionalData.GallerySlot != null)
                    {
                        newSize = new Vector2(50, 60);
                        //backMsg.sizeDelta = newSize;   
                    }
                    else
                    {
                        //backMsg.sizeDelta = new Vector2(150, 100);
                    }

                    rightBorderActor.gameObject.Deactivate();
                    Image storyTellerIcon = leftBorderActor.transform.GetChild(0).GetComponent<Image>();
                    storyTellerIcon.sprite = storyTeller;
                    SetIconStoryTeller(needIconStoryTeller);
                    Debug.Log("StoryTeller installed");
                    break;
            }
        }

        protected void SetOptions(MessageData data)
        {
            for (int i = 0; i < OptionButtons.Length && i < data.optionalData.Branches.Length; i++)
            {
                if (data.optionalData.Branches[i] != null)
                {
                    OptionButtons[i].BranchData = data.optionalData.Branches[i];
                    TextMeshProUGUI textChildren = OptionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                    textChildren.text = OptionButtons[i].BranchData.BranchName;
                }
            }
        }

        protected string SetName(string actorLeft, string actorRight, string storyTeller)
        {
            string nameActor = "";
            switch (Sender)
            {
                case MessageSender.ActorLeft:
                    nameActor = actorLeft;
                    break;
                case MessageSender.ActorRight:
                    nameActor = actorRight;
                    break;
                case MessageSender.StoryTeller:
                    nameActor = storyTeller;
                    break;
            }

            msgNameText.text = nameActor;
            return nameActor;
        }

        private void SetIconStoryTeller(bool needIcon)
        {
            leftBorderActor.gameObject.SetActive(needIcon);
        }
    }
}