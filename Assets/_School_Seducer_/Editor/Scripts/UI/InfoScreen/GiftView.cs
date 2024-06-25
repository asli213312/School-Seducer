using _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class GiftView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image gift;
        [SerializeField] private TextMeshProUGUI count;
        
        public WheelSlotData Data { get; private set; }
        
        public void Render(WheelSlotData giftData, int sameGiftCount)
        {
            Data = giftData;
            
            if (giftData.iconInfo == null) return;
            if (giftData.borderSpriteByColor == null) return;

            background.sprite = giftData.borderSpriteByColor;
            gift.sprite = giftData.iconInfo;

            count.text = sameGiftCount.ToString();
        }
    }
}