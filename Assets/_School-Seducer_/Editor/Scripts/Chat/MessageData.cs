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

        private bool _isBigMessage;
        public MessageSender Sender { get; set; }

        public bool IsBigMessage() => _isBigMessage;

        public void SetBigMessage()
        {
            _isBigMessage = true;
        }

        private bool IsPictureMsg() => optionalData.PictureInsteadMsg != null;
    }
}