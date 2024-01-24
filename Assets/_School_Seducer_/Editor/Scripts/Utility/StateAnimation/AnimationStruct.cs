using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    [Serializable]
    public class AnimationStruct
    {
        public AnimationType type;
        public GameObject gameObject;
        public AnimationMove animationPosition;
        public AnimationRotate animationRotate;
    }
}