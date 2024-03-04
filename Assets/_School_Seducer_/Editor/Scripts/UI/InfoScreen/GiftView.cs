using _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts
{
    public class GiftView : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image gift;
        
        public void Render(WheelSlotData giftData) 
        {
            if (giftData.iconInfo == null) return;
            if (giftData.borderSpriteByColor == null) return;

            background.sprite = giftData.borderSpriteByColor;
            gift.sprite = giftData.iconInfo;
        }
    }
}