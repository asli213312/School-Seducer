using Sirenix.Utilities;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(fileName = "Main Screen", menuName = "Game/Shop/Tabs/MainTab", order = 0)]
    public class ShopTabDataMainScreen : ShopTabDataBase 
    {
    	[SerializeField, SerializeReference] public ShopTypeItemDataBase[] items;	
    	public override IShopItemPurchasable[] ItemsData => items;

        [ContextMenu("Reset Items Sold")]
        private void ResetItemsSold()
        {
            ItemsData.ForEach(x => x.IsSold = false);

            foreach (var item in ItemsData)
            {
                item.IsSold = false;

                if (item is ShopGroupItemData itemGroup)
                {
                    itemGroup.items.ForEach(x => x.IsSold = false);
                }
            }
        }

        [ContextMenu("Lock items Content")]
        private void LockItemsContent()
        {
            foreach (var item in items)
            {
                if (item is IShopItemLockable itemLockable)
                    itemLockable.LockContent();
            }
        }
    }
}