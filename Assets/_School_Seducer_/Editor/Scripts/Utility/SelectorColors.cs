using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public class SelectorColors : MonoBehaviour
    {
        [SerializeField] private Button[] deselectButtons;
        [SerializeField] private Color highlighted;
        [SerializeField] private Button selectButton;

        private ColorBlock DisabledColors => new ColorBlock() 
        {
            normalColor = Color.clear,
            highlightedColor = highlighted,
            pressedColor = Color.clear,
            selectedColor = Color.clear,
            disabledColor = Color.clear,
            colorMultiplier = 1,
            fadeDuration = 0.1f
        };

        private ColorBlock EnabledColors => new ColorBlock()
        {
            normalColor = Color.white,
            highlightedColor = highlighted,
            pressedColor = Color.clear,
            selectedColor = Color.clear,
            disabledColor = Color.clear,
            colorMultiplier = 1,
            fadeDuration = 0.1f
        };

        public void SelectButton() 
        {
        	selectButton.colors = EnabledColors;

        	foreach (var deselectButton in deselectButtons) 
        	{
        		deselectButton.colors = DisabledColors;
        	}
        }
    }
}