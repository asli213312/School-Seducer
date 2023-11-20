using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
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

        protected void SetSenderMsg(Sprite actorRight, Sprite actorLeft, Sprite storyTeller, MessageData data)
        {
            Data = data;
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
                    break;
                
                case MessageSender.ActorLeft:
	                msgText.text = data.Msg;
                    Image leftIcon = leftBorderActor.transform.GetChild(0).GetComponent<Image>();
                    leftIcon.sprite = actorLeft;
                    Debug.Log("Base actor installed");
                    break;
                
                case MessageSender.StoryTeller:
                    msgNameText.alignment = TextAnchor.MiddleCenter;
                    msgText.text = data.Msg;

                    backMsg.Translate(Vector2.right * 0.5f, Space.Self);

                    rightBorderActor.gameObject.Deactivate();
                    Image storyTellerIcon = leftBorderActor.transform.GetChild(0).GetComponent<Image>();
                    storyTellerIcon.sprite = storyTeller;

                    leftBorderActor.gameObject.Activate();
                    break;
            }
            Debug.Log("Right actor installed");
        }
        
	    protected void SetName(string actorLeft, string actorRight, string storyTeller)
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
        }
    }
}