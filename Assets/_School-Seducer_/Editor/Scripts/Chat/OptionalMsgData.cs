using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [System.Serializable]
    public class OptionalMsgData
    {
        public Sprite PictureInsteadMsg;
        public AnimationClip Animation;
        public BranchData[] Branches;
    }
}