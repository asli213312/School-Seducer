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
        protected override IShopItemDataBase Data { get => _data; set => _data = value as ShopItemPremiumCharacterPackData; }
        private ShopItemPremiumCharacterPackData _data;
        
        protected override bool TryBuy()
        {
            _data.characterItem.isSold = true;
            _data.groupItem.isSold = true;
            
            _data.characterItem.view.Buy();
            _data.groupItem.view.Buy();

            return true;
        }
    }
}