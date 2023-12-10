using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class WheelSlot : MonoBehaviour
    {
        [SerializeField] private int index;
        [SerializeField, InlineEditor] private WheelSlotData data;
        public WheelSlotData Data => data;
        
        private Image _image;
        private Sprite _moneySprite;

        public void Initialize(Sprite moneySprite)
        {
            _moneySprite = moneySprite;
            Render();
        }

        public int GetIndex() => index;

        public Vector2 GetProbabilityRange()
        {
            return IsTest() ? data.chanceToGainTest : data.chanceToGain;
        }

        private void OnValidate()
        {
            _image = GetComponent<Image>();
        }

        private void Render()
        {
            if (IsTest())
            {
                _image.sprite = data.slotTypeTest == WheelCategorySlotEnum.Money ? _moneySprite : data.iconTest;
            }
            else
                _image.sprite = data.slotType == WheelCategorySlotEnum.Money ? _moneySprite : data.icon;
        }

        private bool IsTest()
        {
            return data.useTest;
        }
    }
}