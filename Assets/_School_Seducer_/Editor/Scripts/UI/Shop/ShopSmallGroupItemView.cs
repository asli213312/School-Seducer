using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSmallGroupItemView : ShopGroupItemViewBase
    {
        protected override IShopItemDataBase Data {get => data; set => data = value as ShopGroupItemData; }
        protected ShopGroupItemData data;

        protected override void MainRender()
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = data.columnsAmount;

            foreach (var itemData in data.items)
            {
                if (itemData.isSold) continue;
                
                ShopSingleItemGroupViewBase singleItemGroup = Instantiate(itemData.view, content);
                
                Items.Add(singleItemGroup);

                singleItemGroup.BaseRender(itemData);
                singleItemGroup.AdditionalRender();
                
                singleItemGroup.SoldAction += InvokeSoldItem;
            }

            Invoke(nameof(UpdateLayout), 0.1f);
            Invoke(nameof(InitializeItems), 0.1f);
        }

        protected override bool TryBuy()
        {
            data.IsSold = true;
            return true;
        }

        private void OnDestroy()
        {
            foreach (var item in Items)
            {
                item.SoldAction -= InvokeSoldItem;
            }
        }
    }
}