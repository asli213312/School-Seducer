using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    [System.Serializable]
    public class TextWrapper : TextBase
    {
        [SerializeField] private Text textComponent;

        public override string Text {get => textComponent.text; set => textComponent.text = value; }
        public override Color Color { get => textComponent.color; set => textComponent.color = value; }
    }
}