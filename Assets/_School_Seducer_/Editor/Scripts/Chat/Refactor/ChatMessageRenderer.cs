using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEngine;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public class ChatMessageRenderer : MonoBehaviour, IChatMessageRendererModule
    {
        [Inject] private EventManager _eventManager;

        [Header("Data")]
        [SerializeField] private MessageDefaultViewProxy msgDefaultPrefab;
        [SerializeField] private MessagePictureViewProxy msgPicturePrefab;

        private ChatSystem _chatSystem;

        public void InitializeCore(ChatSystem chatSystem)
        {
            _chatSystem = chatSystem;            
        }

        public void RenderMessage(IMessageProxy newMsg, MessageData data, Transform content)
        {
            newMsg.RenderGeneralData(data, _chatSystem.ChatConfig, content);
        }

        public IMessageProxy InstallPrefabMsg(MessageData messageData)
        {
            IMessageProxy chosenPrefab = messageData.optionalData.GallerySlot != null ? msgPicturePrefab : msgDefaultPrefab;

            Debug.Log("chosen prefab");

            IMessageProxy newMsg = Instantiate((MonoBehaviour) chosenPrefab, _chatSystem.ContentMsg).GetComponent<IMessageProxy>();

            return newMsg;
        }
    }
}