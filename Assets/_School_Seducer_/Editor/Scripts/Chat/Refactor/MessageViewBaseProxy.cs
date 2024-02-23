using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public abstract class MessageViewBaseProxy : MonoBehaviour, IMessageBase
    {
        [SerializeField] protected Image leftBorderActor;
        [SerializeField] protected Image rightBorderActor;

        public MessageData Data { get; private set; }
        protected MessageSender Sender;

        protected void SetSenderMsg(MessageData data)
        {
            Data = data;
            Sender = data.Sender;

            switch (data.Sender)
            {
            case MessageSender.ActorRight:
                rightBorderActor.gameObject.Activate();
                    Image rightIcon = rightBorderActor.transform.GetChild(0).GetComponent<Image>();
                    rightIcon.sprite = data.ActorIcon;
                    leftBorderActor.gameObject.Deactivate();
                    Debug.Log("Right actor installed");
                    break;
                
                case MessageSender.ActorLeft:
                    Image leftIcon = leftBorderActor.transform.GetChild(0).GetComponent<Image>();
                    leftIcon.sprite = data.ActorIcon;
                    Debug.Log("Base actor installed");
                    break;
                
                case MessageSender.StoryTeller:
                    RectTransform mainRect = gameObject.GetComponent<RectTransform>();

                    //backMsg.Translate(Vector2.right * (0.15f), Space.Self);

                    rightBorderActor.gameObject.Deactivate();
                    Image storyTellerIcon = leftBorderActor.transform.GetChild(0).GetComponent<Image>();
                    //storyTellerIcon.sprite = storyTeller;
                    //SetIconStoryTeller(needIconStoryTeller);
                    Debug.Log("StoryTeller installed");
                    break;
            }
        }
    }
}