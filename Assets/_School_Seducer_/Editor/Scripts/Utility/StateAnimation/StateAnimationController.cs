using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public class StateAnimationController : MonoBehaviour
    {
        private enum StartType { Start, Delayed}
        [SerializeField] private List<AnimationStruct> animations;
        [SerializeField, Tooltip("When need to start invoke animations step by step")] private StartType startAnimateType;

        private void OnEnable()
        {
            StartCoroutine(Animate());
        }

        private IEnumerator Start()
        {
            if (startAnimateType != StartType.Start) yield break;

            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            foreach (var animation in animations)
            {
                switch (animation.type)
                {
                    case AnimationType.Move: AnimationBase moveBase = animation.animationPosition;
                        moveBase.Initialize(animation.gameObject, this);
                        animation.animationPosition.Invoke();

                        break;
                }

                yield return new WaitForSeconds(2);
            }
        }
    }
}