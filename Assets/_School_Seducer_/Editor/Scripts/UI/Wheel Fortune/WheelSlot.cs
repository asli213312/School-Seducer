using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class WheelSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField, InlineEditor] private WheelSlotData data;
        public WheelSlotData Data => data;
        
        public Image Image { get; private set; }
        private Sprite _moneySprite;

        public void Initialize(WheelSlotData dataSlot, Sprite moneySprite)
        {
            //if (dataSlot != null) data = dataSlot;

            Image = GetComponent<Image>();

            data = dataSlot;
            
            _moneySprite = moneySprite;
            Render();
        }

        private void Render()
        {
            if (IsTest())
            {
                Image.sprite = data.slotTypeTest == WheelCategorySlotEnum.Money ? _moneySprite : data.iconTest;
            }
            else
                Image.sprite = data.slotType == WheelCategorySlotEnum.Money ? _moneySprite : data.icon;
            
            if (data.IsCharacter() == false)
                text.text = data.costExp.ToString();
        }
        
        private bool IsTest()
        {
            return data.useTest;
        }
    }
}