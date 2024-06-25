using System;
using Unity.Properties;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/Default Group", fileName = "Default Group")]
    public class ShopGroupItemData : ShopTypeItemDataBase
    {
        public int columnsAmount;
        public ShopGroupItemViewBase view;
        public ShopSingleItemGroupDataBase[] items;

        protected override IShopItemView itemView => view;
        
        public override void LockContent()
        {
            foreach (var item in items)
            {
                if (item is IShopItemLockable lockableItem)
                    lockableItem.LockContent();
            }
        }
    }
}