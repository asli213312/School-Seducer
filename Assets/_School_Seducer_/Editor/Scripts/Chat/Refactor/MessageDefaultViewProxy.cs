using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class MessageDefaultViewProxy : MessageViewBaseProxy, IMessageProxy
    {
        [SerializeReference, SerializeField] private TextBase msgText;
        [SerializeField] private Image background;
        
        public TextBase MsgText => msgText;
        public GameObject PaddingForward {get; private set;}

        private ChatConfig _chatConfig;

        public void RenderGeneralData(MessageData data, ChatConfig chatConfig, Transform contentMsg)
        {
            _chatConfig = chatConfig;
            
            SetSenderMsg(data);
            AdjustRightActor(contentMsg, Sender);
            SetBlockBackground();
            SetFontColor();

            msgText.Text = data.Msg;
        }

        private void AdjustRightActor(Transform content, MessageSender sender)
        {
            if (sender != MessageSender.ActorRight) return;

            Debug.Log("Parent for right actor installed");

            GameObject parent = CreateParentObject("ParentRight", content);
            GameObject contentRight = CreateChildObject("ContentRight", parent.transform);
            GameObject paddingForward = CreateChildObject("Padding Forward", parent.transform);
            PaddingForward = paddingForward;

            SetSiblingIndexAlongParent(paddingForward.transform, content, parent.transform.GetSiblingIndex() + 1);

            RectTransform parentRect = parent.AddComponent<RectTransform>();
            contentRight.AddComponent<RectTransform>();
            paddingForward.AddComponent<RectTransform>();

            AlignSizeDelta alignMain = gameObject.AddComponent<AlignSizeDelta>();
            alignMain.Initialize(parentRect, gameObject.GetComponent<RectTransform>(), msgText.Text.Length);

            VerticalLayoutGroup speechBubbleLayout = transform.GetChild(1).GetComponent<VerticalLayoutGroup>();
            speechBubbleLayout.padding.left = 18;
            speechBubbleLayout.padding.right = 30;

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

            parent.GetComponent<RectTransform>().sizeDelta =
                rectMain.sizeDelta + new Vector2(0, leftBorderRect.sizeDelta.y / 24);
            contentRight.GetComponent<RectTransform>().sizeDelta = rectMain.sizeDelta;

            rightBorderActor.transform.SetParent(contentRight.transform);
            transform.SetParent(contentRight.transform);
            rightBorderActor.transform.position = parent.transform.position;
            transform.position = parent.transform.position;

            rightBorderRect.Translate(-Vector2.left * 7.03f);
            rectMain.position = rightBorderRect.position;

            SetStretchMode(contentRight.GetComponent<RectTransform>());

            paddingForward.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0.7f);

            void SetStretchMode(RectTransform contentRightRect)
            {
                contentRightRect.anchorMin = new Vector2(0.5f, 1f);
                contentRightRect.anchorMax = new Vector2(0.5f, 1f);

                contentRightRect.pivot = new Vector2(0.5f, 1f);
            }
        }
        
        private void SetPivotPosTopMode(RectTransform leftBorderRect)
        {
            leftBorderRect.anchorMin = new Vector2(0f, 0f);
            leftBorderRect.anchorMax = new Vector2(1f, 1f);

            leftBorderRect.pivot = new Vector2(0.5f, 0.5f);

            leftBorderRect.sizeDelta = new Vector2(leftBorderRect.sizeDelta.x, 50f);
        }

        private void SetBlockBackground()
        {
            Sprite selectedBlock = null;
            
            switch (Sender)
            {
                case MessageSender.ActorLeft: selectedBlock = _chatConfig.leftActorBlockMsg; break;
                case MessageSender.ActorRight: selectedBlock = _chatConfig.rightActorBlockMsg; break;
                case MessageSender.StoryTeller: selectedBlock = _chatConfig.storyTellerBlockMsg; break;
            }

            background.sprite = selectedBlock;
        }
        
        public void SetFontColor()
        {
            Color selectedColor = Color.clear;
            
            switch (Sender)
            {
                case MessageSender.ActorLeft: selectedColor = _chatConfig.leftActorColorMsg; break;
                case MessageSender.ActorRight: selectedColor = _chatConfig.rightActorColorMsg; break;
                case MessageSender.StoryTeller: selectedColor = _chatConfig.storyTellerColorMsg; break;
            }
            
            msgText.Color = selectedColor;
        }
    }
}