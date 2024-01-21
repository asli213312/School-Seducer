using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class StateAnimationController : MonoBehaviour
    {
        private enum StartType { Start, Delayed}
        [SerializeField] private AnimationBase[] animations;
        [SerializeField, Tooltip("When need to start invoke animations step by step")] private StartType startAnimateType;

        private void OnValidate()
        {
            foreach (var animation in animations)
            {
                switch (animation.type)
                {
                    case AnimationType.Move: 
                        animation.animationRotate = null;
                        animation.animationPosition = new();
                        break;
                    
                    case AnimationType.Rotate: 
                        animation.animationRotate = new();
                        animation.animationPosition = null;
                        break;
                }
            }
        }

        private void Start()
        {
            if (startAnimateType != StartType.Start) return;
        }
    }
}