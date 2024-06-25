using Sirenix.Utilities;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(fileName = "Premium Screen", menuName = "Game/Shop/Tabs/PremiumTab", order = 0)]
    public class ShopTabDataPremiumScreen : ShopTabDataBase 
    {
    	[SerializeField, SerializeReference] public ShopItemPremiumCharacterPackData[] items;
        public override IShopItemPurchasable[] ItemsData => items;

        [ContextMenu("Reset Items Sold")]
        private void ResetItemsSold()
        {
            foreach (var item in items)
            {
                item.characterItem.IsSold = false;
                item.groupItem.IsSold = false;

                item.groupItem.items.ForEach(x => x.IsSold = false);
            }
        }
        
        [ContextMenu("Lock items Content")]
        private void LockItemsContent()
        {
            foreach (var item in items)
            {
                item.groupItem.LockContent();
                item.characterItem.LockContent();
            }
        }
    }
}