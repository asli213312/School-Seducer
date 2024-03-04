using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class WinSpinCharacterView : MonoBehaviour
    {
        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Image winSlot;
        [SerializeField] private Image winSlotBack;

        public void Render(WheelSlotData characterData, WheelSlotData slotData)
        {
            portrait.sprite = characterData.extraCharacterPortrait;
            characterName.text = characterData.name;
            winSlot.sprite = slotData.iconInfo;
            winSlotBack.sprite = slotData.borderSpriteByColor;
        }
    }
}