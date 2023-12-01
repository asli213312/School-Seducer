using NaughtyAttributes;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [System.Serializable]
    public class MessageData
    {
        [ShowAssetPreview(32, 32), SerializeField] public Sprite ActorIcon;
        [ResizableTextArea] [HideIf("IsPictureMsg")] public string Msg;
        public OptionalMsgData optionalData;
        [HideIf("IsPictureMsg"), SerializeField] public AudioClip AudioMsg;

        public MessageSender Sender { get; set; }

        private bool _isBigMessage;

        public bool IsBigMessage() => _isBigMessage;

        public void SetBigMessage()
        {
            _isBigMessage = true;
        }

        public bool IsPictureMsg() => optionalData.GallerySlot != null;
    }
}