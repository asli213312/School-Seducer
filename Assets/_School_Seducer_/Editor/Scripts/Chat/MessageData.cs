using NaughtyAttributes;
using OneLine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [System.Serializable]
    public class MessageData
    {
        [ShowAssetPreview(32, 32), SerializeField] public Sprite ActorIcon;
        [ResizableTextArea] [NaughtyAttributes.HideIf("IsPictureMsg")] public string Msg;
        public OptionalMsgData optionalData;
        [NaughtyAttributes.HideIf("IsPictureMsg")] public AudioClip AudioMsg;

        [field: NaughtyAttributes.ReadOnly] public MessageSender Sender { get; set; }

        private bool _isBigMessage;

        public bool IsBigMessage() => _isBigMessage;

        public void SetBigMessage()
        {
            _isBigMessage = true;
        }

        public bool IsPictureMsg() => optionalData.GallerySlot != null;
    }
}