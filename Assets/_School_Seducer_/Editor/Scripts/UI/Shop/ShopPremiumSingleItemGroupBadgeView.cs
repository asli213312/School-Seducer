using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopPremiumSingleItemGroupBadgeView : MonoBehaviour
    {
        [SerializeField] private Image contentImage;
        [SerializeField] private Image back;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI labelText;

        public void Render(ShopPopupItemBadgeBase badgeData, int typeCount)
        {
            contentImage.sprite = badgeData.contentImage;
            back.sprite = badgeData.back;
            icon.sprite = badgeData.icon;
            labelText.text = typeCount + " " + badgeData.label;
        }
    }
}