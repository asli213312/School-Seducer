using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using DialogueEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class MessagePictureView : MessageViewBase, IMessage, IMessageName
    {
        [SerializeField] private RectTransform contentOffset;
        [SerializeField] private Image msgWidePicture;
        [SerializeField] private Image msgSquarePicture;
        
        public string MsgNameText { get; set; }
        public bool PictureInstalled { get; private set; }
        public MessageSender MessageSender { get; set; }
        
        private float _durationSendingPicture;

        public void Initialize(OptionButton[] optionButtons)
        {
            MessageSender = Sender;
            OptionButtons = optionButtons;
        }

        public void SetDurationSendingPicture(float durationSendingPicture)
        {
            _durationSendingPicture = durationSendingPicture;
        }

        public void RenderGeneralData(MessageData data, Sprite actorLeft, Sprite actorRight, Sprite storyTeller, bool needIconStoryTeller)
        {
            SetSenderMsg(actorRight, actorLeft, storyTeller, data, needIconStoryTeller);
            InvokeSendingPicture(data);
            
            Debug.Log("Options were installed");
        }

        public void SetNameActors(string leftActor, string rightActor, string storyTeller)
        {
            string newName = SetName(leftActor, rightActor, storyTeller);
            MsgNameText = newName;
        }

        private void InvokeSendingPicture(MessageData data)
        {
            SendingPicture(data);
        }

        private void SendingPicture(MessageData data)
        {
            //StartCoroutine(SetSendingAnimation());
            SetOptions(data);
            SetPicture(data);
            AdjustRightActor();
            PictureInstalled = true;
            Data.optionalData.GallerySlot.CheckNeedInGallery();
        }

        private IEnumerator SetSendingAnimation()
        {
            msgText.text = $"Sending picture...";
            StartCoroutine(AnimateDots(msgText));

            yield return new WaitForSeconds(_durationSendingPicture);
        }

        private void SetOptions(MessageData data)
        {
            for (int i = 0; i < OptionButtons.Length && i < data.optionalData.Branches.Length; i++)
            {
                if (data.optionalData.Branches[i] != null)
                {
                    OptionButtons[i].BranchData = data.optionalData.Branches[i];
                    Text textChildren = OptionButtons[i].GetComponentInChildren<Text>();
                    textChildren.text = OptionButtons[i].BranchData.BranchName;
                }
            }
        }

        private void SetPicture(MessageData data)
        {
            Sprite picture = null;
            if (data.optionalData.GallerySlot != null) picture = data.optionalData.GallerySlot.Sprite;
            
            if (data.optionalData.GallerySlot.Sprite.IsWideSprite())
            {
                msgWidePicture.sprite = picture;
                msgWidePicture.gameObject.Activate();   
            }
            else
            {
                msgSquarePicture.sprite = picture;
                msgSquarePicture.gameObject.Activate();
            }
            Debug.Log("Picture installed");
        }

        private void AdjustRightActor()
        {
            if (Sender == MessageSender.ActorRight)
            {
                contentOffset.Translate(Vector2.right * 0.568f);
                msgSquarePicture.transform.position = rightBorderActor.transform.position;
            }
        }

        private IEnumerator AnimateDots(Text text)
        {
            while (true)
            {
                if (PictureInstalled)
                {
                    msgText.text = "Picture was sent:";
                    yield break;
                }

                if (!text.text.EndsWith("..."))
                    text.text += "...";
                
                yield return new WaitForSeconds(0.5f);

                string newText;
                for (int i = 0; i < 3; i++)
                {
                    newText = text.text.Substring(0, text.text.Length - 1);
                    text.text = newText;
                    yield return new WaitForSeconds(0.5f);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}