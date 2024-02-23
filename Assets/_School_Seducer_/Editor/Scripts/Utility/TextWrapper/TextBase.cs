using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    public abstract class TextBase : MonoBehaviour
    {
        public abstract string Text { get; set; }
        public abstract Color Color { get; set; }
    }
}