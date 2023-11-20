using NaughtyAttributes;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [System.Serializable]
    public class MessageData
    {
        [ShowAssetPreview(32, 32)] public Sprite ActorIcon;
        [ResizableTextArea] [HideIf("IsPictureMsg")] public string Msg;
        public OptionalMsgData optionalData;

        public MessageSender Sender { get; set; }

        private bool IsPictureMsg() => optionalData.PictureInsteadMsg != null;
    }
}