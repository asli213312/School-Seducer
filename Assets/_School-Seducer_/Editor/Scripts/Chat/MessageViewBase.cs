using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public abstract class MessageViewBase : MonoBehaviour
    {
        [SerializeField] protected Text msgNameText;
        [SerializeField] protected Text msgText;
        [SerializeField] protected Image leftBorderActor;
        [SerializeField] protected Image rightBorderActor;
        [SerializeField] protected RectTransform backMsg;

        protected MessageData Data;
        protected OptionButton[] OptionButtons;
        protected MessageSender Sender;

        private float _maxWidth;
        protected bool IsBigMessage;
        protected bool IsVeryBigMessage;

        protected void SetSenderMsg(Sprite actorRight, Sprite actorLeft, Sprite storyTeller, MessageData data, bool needIconStoryTeller)
        {
            Data = data;
            Sender = data.Sender;

            switch (data.Sender)
            {
            case MessageSender.ActorRight:
                    msgNameText.alignment = TextAnchor.UpperRight;
                    msgText.text = data.Msg;

                    backMsg.Translate(Vector2.right * 1f, Space.Self);

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
                    msgNameText.alignment = TextAnchor.UpperCenter;
                    msgText.text = data.Msg;

                    //backMsg.Translate(Vector2.right * 0.5f, Space.Self);

                    Vector2 newSize; 
                    if (data.optionalData.GallerySlot != null)
                    {
                        newSize = new Vector2(50, 60);
                        backMsg.sizeDelta = newSize;   
                    }
                    else
                    {
                        backMsg.sizeDelta = new Vector2(250, 50);
                    }

                    rightBorderActor.gameObject.Deactivate();
                    Image storyTellerIcon = leftBorderActor.transform.GetChild(0).GetComponent<Image>();
                    storyTellerIcon.sprite = storyTeller;
                    SetIconStoryTeller(needIconStoryTeller);
                    Debug.Log("StoryTeller installed");
                    break;
            }

            if (Data.optionalData.GallerySlot != null && Sender != MessageSender.StoryTeller)
            {
                Vector2 offsetY = new Vector2(0, 30);
                backMsg.sizeDelta = new Vector2(backMsg.sizeDelta.x, msgText.preferredHeight);
                backMsg.sizeDelta += offsetY;
            }
            else if (Sender != MessageSender.StoryTeller)
            {
                msgText.alignment = TextAnchor.UpperLeft;
                
                RectTransform msgRect = msgText.GetComponent<RectTransform>();
                RectTransform nameMsgRect = msgNameText.GetComponent<RectTransform>();
                RectTransform backMsgRect = backMsg.GetComponent<RectTransform>();
                RectTransform baseRect = gameObject.GetComponent<RectTransform>();
                RectTransform actorRect = leftBorderActor.GetComponent<RectTransform>();

                FreezeScale leftBorderFreeze = leftBorderActor.GetComponent<FreezeScale>();
                Image initialLeftBorder = leftBorderActor.GetComponent<Image>();

                if (msgText.preferredHeight > backMsgRect.sizeDelta.y)
                {
                    if (msgText.text.Length > 200)
                    {
                        IsVeryBigMessage = true;    
                    }
                    else if (msgText.text.Length > 100)
                    {
                        IsBigMessage = true;
                    }
                    
                    Debug.Log("IsBigMessage in base: " + IsBigMessage);
                    float heightDifference = msgText.preferredHeight - backMsgRect.sizeDelta.y;

                    // Увеличиваем высоту backMsg
                    backMsgRect.sizeDelta = new Vector2(backMsgRect.sizeDelta.x, backMsgRect.sizeDelta.y + heightDifference / 1.1f);

                    Vector3 initActorSize = actorRect.localScale;
                    Vector2 initBaseSize = baseRect.sizeDelta;
                    baseRect.sizeDelta = new Vector2(baseRect.sizeDelta.x, baseRect.sizeDelta.y + heightDifference / 1.3f);
                    msgRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, backMsgRect.sizeDelta.y);
                    
                    //nameMsgRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, (heightDifference - 65) / 3.5f);

                    Vector2 scaleFactor = new Vector2(
                        baseRect.sizeDelta.x != 0 ? (baseRect.sizeDelta.x / initBaseSize.x): 1f,
                        baseRect.sizeDelta.y != 0 ? (baseRect.sizeDelta.y / initBaseSize.y) * 0.78f : 1f
                    );
                    
                    actorRect.localScale = new Vector3(
                        initActorSize.x / scaleFactor.x,
                        initActorSize.y / scaleFactor.y,
                        initActorSize.z
                    );
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