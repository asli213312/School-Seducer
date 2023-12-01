using _School_Seducer_.Editor.Scripts.UI;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [System.Serializable]
    public class OptionalMsgData
    {
        public GallerySlotData GallerySlot;
        public AnimationClip Animation;
        public BranchData[] Branches;
    }
}