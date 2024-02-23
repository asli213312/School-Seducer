using TMPro;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Utility
{
    [System.Serializable]
    public class TextMeshProWrapper : TextBase
    {
        [SerializeField] private TextMeshProUGUI textMeshProComponent;

        public override string Text {get => textMeshProComponent.text; set => textMeshProComponent.text = value; }
        public override Color Color {get => textMeshProComponent.color; set => textMeshProComponent.color = value; }
    }
}