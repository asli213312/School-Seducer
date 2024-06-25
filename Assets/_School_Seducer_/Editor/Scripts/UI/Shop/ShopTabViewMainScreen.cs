using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopTabViewMainScreen : ShopTabViewBase
    {
        [Serializable]
        private class SingleItemGroupContainer 
        {
            public ShopSingleItemGroupDataBase data;
            public ShopSingleItemGroupViewBase view;
        }

    	[SerializeField] private ShopTabDataMainScreen data;
        [SerializeField] private SingleItemGroupContainer[] runtimeShopItems;
        public override ShopTabDataBase Data => data;

        protected override void InitializeTab() 
        {
            RenderRuntimeItems();
        }

    	protected override void ContentRender() 
    	{
            RenderTabItems();
    	}

        private void RenderRuntimeItems() 
        {
            foreach (var item in runtimeShopItems) 
            {
                item.view.BaseRender(item.data);

                CurrentItems.Add(item.view);

                if (item.view is IShopItemSoldableHardItem itemHardSoldable) 
                {
                    itemHardSoldable.SoldAction += soldableItem => InvokeItemTryBuyEvent(item.view.SingleData);
                    itemHardSoldable.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                    itemHardSoldable.SoldSuccessAction += InvokeItemBuySuccessEvent;
                }
            }
        }

        private void RenderTabItems() 
        {
            foreach (var shopItem in data.items)
            {
                if (shopItem.IsSold) continue;
                
                IShopItemView itemView = Instantiate(shopItem.View as ShopTypeItemViewBase, scroller.content);
                itemView.BaseRender(shopItem);
                
                CurrentItems.Add(itemView);

                if (itemView is IShopItemSoldable soldableItem)
                {
                    if (soldableItem is IShopItemSoldableHardItem itemHardSoldable) 
                    {
                        itemHardSoldable.SoldAction += soldableItem => InvokeItemTryBuyEvent(shopItem);
                        itemHardSoldable.SoldAction += soldableItem => MainRender();
                        itemHardSoldable.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                        itemHardSoldable.SoldSuccessAction += InvokeItemBuySuccessEvent;
                    }
                    else if (soldableItem is IShopItemSoldableSoftItem itemSoftSoldable) 
                    {
                        itemSoftSoldable.SoldAction += soldableItem => MainRender();   
                        itemSoftSoldable.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                    }
                }   

                if (itemView is ShopGroupItemViewBase groupView) 
                {
                    foreach (var singleItemView in groupView.Items) 
                    {
                        if (singleItemView is IShopItemSoldableHardItem singleHardItemView) 
                        {
                            singleHardItemView.SoldAction += soldableItem => InvokeItemTryBuyEvent(singleItemView.SingleData);
                            singleHardItemView.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                            singleHardItemView.SoldSuccessAction += InvokeItemBuySuccessEvent;
                        }
                    }
                }
            }
        }

        protected override void UnRegisterContent()
        {
	        foreach (var item in CurrentItems)
	        {
                if (item is IShopItemSoldable soldableItem)
                {
                    if (soldableItem is IShopItemSoldableSoftItem itemSoftSoldable) 
                    {
                        itemSoftSoldable.SoldAction -= itemSoldable => MainRender();
                        itemSoftSoldable.SoldAction -= itemSoldable => InvokeItemTryBuyInvokedEvent();
                    }
                    else if (soldableItem is IShopItemSoldableHardItem itemHardSoldable) 
                    {
                        itemHardSoldable.SoldAction -= itemSoldable => MainRender();
                        itemHardSoldable.SoldAction -= soldableItem => InvokeItemTryBuyEvent(item.MainData);
                        itemHardSoldable.SoldAction -= soldableItem => InvokeItemTryBuyInvokedEvent();
                        itemHardSoldable.SoldSuccessAction -= InvokeItemBuySuccessEvent;
                    }   
                }

                if (item is ShopGroupItemViewBase groupView) 
                {
                    foreach (var singleItemView in groupView.Items) 
                    {
                        if (singleItemView is IShopItemSoldableHardItem singleHardItemView) 
                        {   
                            singleHardItemView.SoldAction -= soldableItem => InvokeItemTryBuyEvent(singleItemView.SingleData);
                            singleHardItemView.SoldAction -= soldableItem => InvokeItemTryBuyInvokedEvent();
                            singleHardItemView.SoldSuccessAction -= InvokeItemBuySuccessEvent;
                        }
                    }
                }
	        }
        }
    }
}