using System.Dynamic;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public abstract class AnimationBase
    {
        protected GameObject Target;
        protected MonoBehaviour Behavior;
        protected Vector3 StartPosition;

        public void Initialize(GameObject target, MonoBehaviour behaviour)
        {
            Target = target;
            Behavior = behaviour;

            if (Target.transform is RectTransform)
                StartPosition = Target.GetComponent<RectTransform>().anchoredPosition;
            else
                StartPosition = Target.transform.position;
        }
        public abstract void Invoke();
    }
}