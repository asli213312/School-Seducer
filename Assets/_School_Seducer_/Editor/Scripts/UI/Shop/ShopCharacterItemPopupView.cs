using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [Serializable]
    public class ShopPopupItemBadgeBase
    {
        public string label;
        public Sprite icon;
        public Sprite back;
        public Sprite contentImage;
    }

    [Serializable]
    public class ShopPopupSingleGroupItemBadge : ShopPopupItemBadgeBase
    {
        public ShopSingleItemGroupDataBase[] types;   
    }
    
    [Serializable]
    class ShopPopupItemBadge : ShopPopupItemBadgeBase
    {
        public ShopSingleItemDataBase[] types;   
    }

    public class ShopCharacterItemPopupView : ShopItemPopupViewBase
    {
        [SerializeField] protected Image characterPreview;
        [SerializeField] protected ShopPremiumSingleItemGroupBadgeView singleItemBadgeView;
        [SerializeField] protected RectTransform groupItemsContent;
        [SerializeField] protected TextMeshProUGUI description;
        [SerializeField] protected TextMeshProUGUI characterNameText;
        
        [Header("Options")]
        [SerializeField] private float badgesSpacingX;

        [Header("Badges")]
        [SerializeField, SerializeReference] protected ShopPopupSingleGroupItemBadge[] badges;
        
        protected virtual IShopItemDataBase BadgeItem => _data;
        protected override List<IShopItemView> ItemViews { get; set; }
        protected override IShopItemDataBase ItemData => _data;
        private IShopItemCharacterData _data;
        
        private float _badgesSpacing;

        protected override bool IsValidType<T>(T itemData)
        {
            return itemData is IShopItemCharacterData;
        }

        protected override void OnRender<T>(T data, List<IShopItemView> shopItemViews)
        {
            if (data is not IShopItemCharacterData) 
            {
                Debug.LogError("<color=red>SHOP</color> type not correct to create popup! " + data + "but need type: IShopItemCharacterData");
                return;
            }

            _data = data as IShopItemCharacterData;
            ItemViews = shopItemViews;
            
            BaseRender(data, shopItemViews);

            _badgesSpacing = badgesSpacingX;

            characterPreview.sprite = _data.CharacterData.info.portrait;
            characterNameText.text = _data.CharacterData.name;
            
            description.text = _data.Description;
            
            RenderBadges(BadgeItem);
        }

        protected void BaseRender(IShopItemDataBase data, List<IShopItemView> shopItemViews) => base.OnRender(data, shopItemViews);

        protected virtual bool IsValidBadgeType<T>(T itemData) where T : IShopItemDataBase
        {
            return itemData is IShopItemCharacterData;
        }

        protected virtual bool IsRestrictedToCreateBadge(IShopItemDataBase[] types)
        {
            return types.All(x => x.GetType() != BadgeItem.GetType());
        }

        protected virtual bool CanAddBadge(IShopItemDataBase badgeType, IShopItemDataBase itemData)
        {
            return badgeType.GetType() == itemData.GetType();
        }
        
        protected void RenderBadges<T>(T itemData) where T : IShopItemDataBase
        {
            if (IsValidBadgeType(itemData) == false) return;

            if (BadgeItem is ShopSingleItemInfoCharacter badgeItemCharacter) 
            {
                CreateGroupItemBadge(badges[0], badgeItemCharacter.CharacterData.allConversations.Count);
            }
            
            foreach (var badge in badges)
            {
                var itemsByTypeBadge = GetItemsByBadgeTypes(badge.types);
                
                if (itemsByTypeBadge == null) continue;
                CreateGroupItemBadge(badge, itemsByTypeBadge.Count);
            }
        }
        
        private List<IShopItemDataBase> GetItemsByBadgeTypes(IShopItemDataBase[] types)
        {
            if (IsRestrictedToCreateBadge(types)) return null;
            
            List<IShopItemDataBase> itemsByType = new();

            foreach (var badgeType in types)
            {
                if (CanAddBadge(badgeType, BadgeItem) == false) continue;
                
                itemsByType.Add(BadgeItem);
            }

            return itemsByType;
        }

        protected void CreateGroupItemBadge(ShopPopupItemBadgeBase badge, int amountTypeItems)
        {
            var badgeView = Instantiate(singleItemBadgeView, groupItemsContent.transform);
            badgeView.transform.position += new Vector3(_badgesSpacing, 0);
            badgeView.Render(badge, amountTypeItems);

            _badgesSpacing += badgesSpacingX;
        }
    }
}