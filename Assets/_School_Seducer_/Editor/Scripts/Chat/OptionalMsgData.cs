using _School_Seducer_.Editor.Scripts.UI;
using UnityEngine;
using Sirenix.OdinInspector;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [System.Serializable]
    public class OptionalMsgData
    {
        public GallerySlotData GallerySlot;
        public BranchData[] Branches;
        public ContainerIcon otherActorIcon;
    }
    
    [System.Serializable]
    public class ContainerIcon
    {
        public MessageSender sender;
        public Sprite icon;
    }
}