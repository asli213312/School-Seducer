using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.UI;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class MessagePictureViewProxy : MessageViewBaseProxy, IMessageProxy, IMessageCondition
    {
        [SerializeField] private RectTransform contentOffset;
        [SerializeField] private Image msgWidePicture;
        [SerializeField] private Image msgSquarePicture;
        
        public Image CurrentImage { get; private set; }
        public string MsgNameText { get; set; }
        public bool PictureInstalled { get; private set; }

        private float _durationSendingPicture;

        public void RenderGeneralData(MessageData data, ChatConfig chatConfig, Transform contentMsg)
        {
            SetSenderMsg(data);
            InvokeSendingPicture(data);
        }

        public bool CheckCondition() => PictureInstalled == false;

        public void ResolveCondition()
        {
            StartCoroutine(WaitUntilPictureInstalled()); 

            IEnumerator WaitUntilPictureInstalled()
            {
                yield return new WaitForSeconds(1);
            }
        }

        private void InvokeSendingPicture(MessageData data)
        {
            SendingPicture(data);
        }

        private void SendingPicture(MessageData data)
        {
            //StartCoroutine(SetSendingAnimation());
            //SetOptions(data);
            SetPictureOrAnimation(data);
            AdjustRightActor();
            PictureInstalled = true;
            Data.optionalData.GallerySlot.CheckNeedInGallery();
        }

        private void SetPictureOrAnimation(MessageData data)
        {
            Sprite picture = data.optionalData.GallerySlot.Sprite;
            Image selectedImage = null;

            if (data.optionalData.GallerySlot.Sprite.IsWideSprite())
            {
                msgWidePicture.sprite = picture;
                msgWidePicture.gameObject.Activate();

                CurrentImage = msgWidePicture;
                selectedImage = msgWidePicture;
            }
            else
            {
                msgSquarePicture.sprite = picture;
                msgSquarePicture.gameObject.Activate();
                
                CurrentImage = msgSquarePicture;
                selectedImage = msgSquarePicture;
            }

            if (data.optionalData.GallerySlot.animation != null)
            {
                contentOffset.GetComponentInChildren<SkeletonAnimation>().skeletonDataAsset = data.optionalData.GallerySlot.animation;
                selectedImage.GetComponent<OpenContentSprite>().enabled = false;
            }
            else
                selectedImage.GetComponent<OpenContentAnimation>().enabled = false;
            
            Debug.Log("Picture installed");
        }

        private void AdjustRightActor()
        {
            if (Sender == MessageSender.ActorRight)
            {
                contentOffset.Translate(Vector2.right * 1.757f);
                msgSquarePicture.transform.position = rightBorderActor.transform.position;
            }
        }
    }
}