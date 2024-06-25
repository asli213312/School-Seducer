using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [RequireComponent(typeof(LayoutElement))]
    public class ShopItemPremiumCharacterPackView : ShopSingleItemViewBase
    {
        [SerializeField] private Image clickableImage;
        [SerializeField] private HorizontalLayoutGroup horizontalPackLayout;
        [SerializeField] private LayoutElement layout;
        protected override IShopItemDataBase Data { get => data; set => data = value as ShopItemPremiumCharacterPackData; }
        private ShopItemPremiumCharacterPackData data;
        
        protected override bool TryBuy()
        {
            data.characterItem.isSold = true;
            data.groupItem.isSold = true;

            return true;
        }
    }
}