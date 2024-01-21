using UnityEditor;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public abstract class StateAnimationBase : MonoBehaviour
    {
        protected Vector3 Position {get => transform.position; set => transform.position = value;}
        protected Vector3 Rotation {get => transform.rotation.eulerAngles; set => transform.rotation = Quaternion.Euler(new Vector3());}
        protected Vector3 Scale {get => transform.localScale; set => transform.localScale = value; }

        protected abstract void Initialize();
    }
}