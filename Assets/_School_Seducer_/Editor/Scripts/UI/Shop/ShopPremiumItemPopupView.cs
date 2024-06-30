using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopPremiumItemPopupView : ShopCharacterItemPopupView
    {
        protected override List<IShopItemView> ItemViews { get; set; }
        protected override IShopItemDataBase BadgeItem => _badgeData;
        protected override IShopItemDataBase ItemData => _data;
        private ShopItemPremiumCharacterPackData _data;
        private ShopGroupItemData _badgeData;

        protected override bool IsValidType<T>(T itemData)
        {
            return itemData is ShopItemPremiumCharacterPackData;
        }

        protected override bool IsValidBadgeType<T>(T itemData)
        {
            return itemData is ShopGroupItemData;
        }

        protected override void OnRender<T>(T data, List<IShopItemView> shopItemViews)
        {
            if (data is not ShopItemPremiumCharacterPackData) 
            {
                Debug.LogError("<color=red>SHOP</color> type not correct to create popup! " + data + "but need type: ShopItemPremiumCharacterPackData");
                return;
            }

            _data = data as ShopItemPremiumCharacterPackData;
            _badgeData = _data.groupItem;
            ItemViews = shopItemViews;

            BaseRender(_data, shopItemViews);

            characterPreview.sprite = _data.characterItem.characterData.info.portrait;
            characterNameText.text = _data.characterItem.characterData.name;

            CreateGroupItemBadge(badges[0], _data.characterItem.characterData.allConversations.Count);
            description.text = _data.description;
            
            RenderBadges(_badgeData);
        }

        protected override bool IsRestrictedToCreateBadge(IShopItemDataBase[] types)
        {
            bool isRestricted = _badgeData.items.All(x => types.All(y => x.GetType() != y.GetType()));

            if (isRestricted) 
            {
                Debug.Log("Is restricted to create popup badge");
                return true;
            }
             
            return false;
        }

        protected override bool CanAddBadge(IShopItemDataBase badgeType, IShopItemDataBase itemData)
        {
            bool canAdd = _badgeData.items.Any(x => x.GetType() == badgeType.GetType());

            if (canAdd == false) 
            {
                Debug.Log("Can't add badge");
                return false;
            }
            
            return true;
        }
    }
}