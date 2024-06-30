using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopTabViewPremiumContent : ShopTabViewBase
    {
        [SerializeField] private ShopTabDataPremiumScreen data;
        public override ShopTabDataBase Data => data;

        protected override void ContentRender() 
    	{
            foreach (var shopItem in data.items)
            {
                if (shopItem.groupItem.isSold && shopItem.characterItem.IsSold)
                {
                    shopItem.IsSold = true;
                    continue;
                }

                if (shopItem.characterItem.IsSold == false)
                {
                    ShopItemViewCharacter spawnedCharacterView = Instantiate(shopItem.characterItem.view, scroller.content).GetComponent<ShopItemViewCharacter>();
                    spawnedCharacterView.gameObject.GetComponent<LayoutElement>().minWidth = 398.12f;
                    spawnedCharacterView.BaseRender(shopItem.characterItem);
                    
                    if (spawnedCharacterView is IShopItemSoldable soldableCharacterItem)
                    {
                        if (soldableCharacterItem is IShopItemSoldableHardItem characterHardItemView) 
                        {
                            characterHardItemView.SoldAction += soldableItem => InvokeItemTryBuyEvent(shopItem.characterItem);
                            characterHardItemView.SoldAction += soldableItem => MainRender();
                            characterHardItemView.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                        } 
                        
                        if (soldableCharacterItem is IShopItemSoldableSoftItem soldableCharacter) 
                        {
                            soldableCharacter.SoldAction += soldableItem => MainRender();
                            soldableCharacter.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                        }
                    }

                    CurrentItems.Add(spawnedCharacterView);
                }

                if (shopItem.groupItem.IsSold) continue;
                
                ShopGroupItemViewBase groupView = Instantiate(shopItem.groupItem.view, scroller.content);
                groupView.BaseRender(shopItem.groupItem);

                if (groupView is IShopItemSoldable soldableGroupItem)
                {
                    switch (soldableGroupItem)
                    {
                        case IShopItemSoldableSoftItem soldableGroupView:
                            soldableGroupView.SoldAction += soldableItem => MainRender();
                            soldableGroupView.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                            break;
                        case IShopItemSoldableHardItem groupSoldableHard:
                            groupSoldableHard.SoldAction += soldableItem => InvokeItemTryBuyEvent(shopItem.groupItem);
                            groupSoldableHard.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                            groupSoldableHard.SoldAction += soldableItem => MainRender();
                            break;
                    }
                }

                foreach (var singleItemView in groupView.Items) 
                {
                    if (singleItemView is IShopItemSoldable soldableSingleItem)
                    {
                        if (soldableSingleItem is IShopItemSoldableHardItem singleSoldableHardItem) 
                        {
                            //singleSoldableHardItem.SoldAction += soldableItem => InvokeItemTryBuyEvent(singleItemView.SingleData);
                            //singleSoldableHardItem.SoldAction += soldableItem => InvokeItemTryBuyInvokedEvent();
                        }   
                    }
                }
            
                CurrentItems.Add(groupView);

                //GameObject cardCharacterWrapper = Instantiate(new GameObject("cardCharacterWrapper"), scroller.content);
                //HorizontalLayoutGroup cardHorizontalGroup = cardCharacterWrapper.AddComponent<HorizontalLayoutGroup>();
                //LayoutElement cardLayout = cardCharacterWrapper.AddComponent<LayoutElement>();
                //cardLayout.minWidth = 400;
            }
    	}

        protected override void UnRegisterContent()
        {
            foreach (var item in CurrentItems)
            {
                if (item is IShopItemSoldable soldableItem) 
                {
                    if (soldableItem is IShopItemSoldableHardItem hardItem) 
                    {
                        hardItem.SoldAction -= soldableItem => InvokeItemTryBuyEvent(item.MainData);
                        hardItem.SoldAction -= soldableItem => InvokeItemTryBuyInvokedEvent();
                        hardItem.SoldAction -= soldableItem => MainRender();
                    }
                    else if (soldableItem is IShopItemSoldableSoftItem softItem)
                    {
                        softItem.SoldAction -= soldableItem => MainRender();
                        softItem.SoldAction -= soldableItem => InvokeItemTryBuyInvokedEvent();
                    }
                }

                if (item is ShopGroupItemViewBase groupView) 
                {
                    foreach (var singleItemView in groupView.Items) 
                    {
                        if (singleItemView is IShopItemSoldableHardItem soldableSingleHardItem) 
                        {
                            //soldableSingleHardItem.SoldAction -= soldableItem => InvokeItemTryBuyEvent(singleItemView.SingleData);
                            //soldableSingleHardItem.SoldAction -= soldableItem => InvokeItemTryBuyInvokedEvent();
                        }
                    }
                }
            }
        }
    }
}