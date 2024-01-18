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

        protected void AdjustStoryTeller(Transform content, MessageSender sender)
        {
            if (sender != MessageSender.StoryTeller) return;

            GameObject parent = CreateParentObject("Parent_StoryTeller", content);

            RectTransform rectParent = parent.AddComponent<RectTransform>();
            RectTransform mainRect = GetComponent<RectTransform>();
            
            parent.transform.position = gameObject.transform.position;

            rectParent.pivot = new Vector2(1, 0.5f);

            rectParent.sizeDelta = new Vector2(3.4f, 0);
            
            AlignSizeDelta alignMain = gameObject.AddComponent<AlignSizeDelta>();
            alignMain.Initialize(rectParent, gameObject.GetComponent<RectTransform>(), msgText.text.Length);

            gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom = 10;

            mainRect.pivot = new Vector2(0.5f, 1f);

            int indexMessage = gameObject.transform.GetSiblingIndex();
            parent.transform.SetSiblingIndex(indexMessage - 1);

            gameObject.transform.SetParent(parent.transform);
        }

        protected void AdjustRightActor(Transform content, MessageSender sender)
        {
            if (sender != MessageSender.ActorRight) return;

            Debug.Log("Parent for right actor installed");

            GameObject parent = CreateParentObject("ParentRight", content);
            GameObject contentRight = CreateChildObject("ContentRight", parent.transform);
            GameObject paddingForward = CreateChildObject("Padding Forward", parent.transform);

            SetSiblingIndexAlongParent(paddingForward.transform, content, parent.transform.GetSiblingIndex() + 1);

            RectTransform parentRect = parent.AddComponent<RectTransform>();
            contentRight.AddComponent<RectTransform>();
            paddingForward.AddComponent<RectTransform>();

            AlignSizeDelta alignMain = gameObject.AddComponent<AlignSizeDelta>();
            alignMain.Initialize(parentRect, gameObject.GetComponent<RectTransform>(), msgText.text.Length);

            SetPivotPosTopMode(leftBorderActor.GetComponent<RectTransform>());

            AdjustSizesAndPositions(parent, contentRight, paddingForward);
        }

        private GameObject CreateParentObject(string name, Transform parent)
        {
            GameObject parentObject = new GameObject(name);
            parentObject.transform.SetParent(parent);
            return parentObject;
        }

        private GameObject CreateChildObject(string name, Transform parent)
        {
            GameObject childObject = new GameObject(name);
            childObject.transform.SetParent(parent);
            return childObject;
        }

        private void SetSiblingIndexAlongParent(Transform target, Transform parent, int index)
        {
            target.SetParent(parent);
            target.SetSiblingIndex(index);
        }

        private void AdjustSizesAndPositions(GameObject parent, GameObject contentRight, GameObject paddingForward)
        {
            RectTransform leftBorderRect = leftBorderActor.GetComponent<RectTransform>();
            RectTransform rightBorderRect = rightBorderActor.GetComponent<RectTransform>();
            RectTransform rectMain = gameObject.GetComponent<RectTransform>();

            SetPivotPosTopMode(leftBorderRect);

            parent.GetComponent<RectTransform>().sizeDelta = rectMain.sizeDelta + new Vector2(0, leftBorderRect.sizeDelta.y / 24);
            contentRight.GetComponent<RectTransform>().sizeDelta = rectMain.sizeDelta;

            rightBorderActor.transform.SetParent(contentRight.transform);
            transform.SetParent(contentRight.transform);
            rightBorderActor.transform.position = parent.transform.position;
            transform.position = parent.transform.position;

            rightBorderRect.Translate(-Vector2.left * 7.03f);
            rectMain.position = rightBorderRect.position;

            SetStretchMode(contentRight.GetComponent<RectTransform>());

            paddingForward.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0.7f);
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