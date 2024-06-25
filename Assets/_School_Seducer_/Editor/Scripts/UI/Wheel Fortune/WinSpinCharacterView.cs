using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class WinSpinCharacterView : MonoBehaviour
    {
        private GlobalSelectors _globalSelector;

        [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Image winSlot;
        [SerializeField] private Image winSlotBack;
        [SerializeField] private Button selectButton;

        public WheelSlotData SlotData { get; private set; }

        public void Initialize(GlobalSelectors globalSelector) 
        {
            _globalSelector = globalSelector;
        }

        public void Render(WheelSlotData characterData, WheelSlotData slotData)
        {
            SlotData = slotData;

            portrait.sprite = characterData.extraCharacterPortrait;
            characterName.text = characterData.name;
            winSlot.sprite = slotData.iconInfo;
            winSlotBack.sprite = slotData.borderSpriteByColor;

            selectButton.onClick.AddListener(() =>_globalSelector.SelectGift(transform));
        }

        private void OnDestroy() 
        {
            selectButton.onClick.RemoveListener(() => _globalSelector.SelectGift(transform));
        }
    }
}