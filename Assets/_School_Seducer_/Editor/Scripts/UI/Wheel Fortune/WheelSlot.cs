using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class WheelSlot : MonoBehaviour
    {
        [SerializeField, InlineEditor] private WheelSlotData data;
        public WheelSlotData Data => data;
        
        private Image _image;
        private Sprite _moneySprite;

        public void Initialize(WheelSlotData dataSlot, Sprite moneySprite)
        {
            //if (dataSlot != null) data = dataSlot;

            _image = GetComponent<Image>();
            
            data = dataSlot;
            
            _moneySprite = moneySprite;
            Render();
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

        public Sprite GetCurrentIcon() => IsTest() ? data.iconTest : data.icon;

        public int GetCostExp() => IsTest() ? data.costExpTest : data.costExp;

        public Vector2 GetProbabilityRange()
        {
            return IsTest() ? data.chanceToGainTest : data.chanceToGain;
        }

        private bool IsTest()
        {
            return data.useTest;
        }
    }
}