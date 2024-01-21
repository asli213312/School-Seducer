using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    [Serializable]
    public class AnimationBase
    {
        public AnimationType type;
        public GameObject gameObject;
        public AnimationMove animationPosition;
        public AnimationRotate animationRotate;
    }
    
    [Serializable]
    public class AnimationMove
    {
        public Vector3 position;
    }

    [Serializable]
    public class AnimationRotate
    {
        public Vector3 rotation;
    }
}