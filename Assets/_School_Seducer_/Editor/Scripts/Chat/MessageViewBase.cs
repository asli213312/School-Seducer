using System.Security.Claims;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI;
using _School_Seducer_.Editor.Scripts.Utility;
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
                    msgNameText.alignment = TextAnchor.UpperRight;
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
                    msgNameText.alignment = TextAnchor.UpperCenter;
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

        protected void AdjustStoryTeller(Transform content)
        {
            GameObject parent = new GameObject("ParentStoryTeller");
            parent.transform.SetParent(content);
            gameObject.transform.SetParent(parent.transform);
        }

        protected void AdjustRightActor(Transform content, MessageSender sender)
        {
            if (sender != MessageSender.ActorRight) return;
            
            Debug.Log("Parent for right actor installed");
            GameObject parent = new GameObject("ParentRight");
            GameObject contentRight = new GameObject("ContentRight");
            parent.transform.SetParent(content);
            contentRight.transform.SetParent(parent.transform);

            RectTransform leftBorderRect = leftBorderActor.GetComponent<RectTransform>();
            RectTransform rightBorderRect = rightBorderActor.GetComponent<RectTransform>();
            RectTransform rectMain = gameObject.GetComponent<RectTransform>();
            RectTransform contentRightRect = contentRight.AddComponent<RectTransform>();
            RectTransform parentRect = parent.AddComponent<RectTransform>();
            AlignSizeDelta alignMain = gameObject.AddComponent<AlignSizeDelta>();
            alignMain.Initialize(parentRect, rectMain, msgText.text.Length);

            SetPivotPosTopMode(leftBorderRect);

            parentRect.sizeDelta = rectMain.sizeDelta + new Vector2(0, leftBorderRect.sizeDelta.y / 24);
            contentRightRect.sizeDelta = rectMain.sizeDelta;
            //contentRightRect.localPosition = new Vector3(3, 0, contentRightRect.position.z);
            rightBorderActor.transform.SetParent(contentRight.transform);
            transform.SetParent(contentRight.transform);
            rightBorderActor.transform.position = parent.transform.position;
            transform.position = parent.transform.position;

            rightBorderRect.Translate(-Vector2.left * 5.83f);
            rectMain.position = rightBorderRect.position;

            SetStretchMode(contentRightRect);
        }

        private void SetPivotPosTopMode(RectTransform leftBorderRect)
        {
            leftBorderRect.anchorMin = new Vector2(0f, 0f);
            leftBorderRect.anchorMax = new Vector2(1f, 1f);

            leftBorderRect.pivot = new Vector2(0.5f, 0.5f);

            leftBorderRect.sizeDelta = new Vector2(leftBorderRect.sizeDelta.x, 50f);
        }

        private void SetStretchMode(RectTransform contentRightRect)
        {
            contentRightRect.anchorMin = new Vector2(0.5f, 1f);
            contentRightRect.anchorMax = new Vector2(0.5f, 1f);

            contentRightRect.pivot = new Vector2(0.5f, 1f);
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