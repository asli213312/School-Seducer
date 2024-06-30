using System;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopTabViewBase : MonoBehaviour
    {
        [Inject] private DiContainer _diContainer;

        [SerializeField] protected ScrollRect scroller;
        [SerializeField] protected Button buttonTab;
        public abstract ShopTabDataBase Data { get; }

        public readonly List<IShopItemView> CurrentItems = new();

        public Action<ShopTabViewBase> TabSelectedAction;
        public event Action ItemTryBuyInvokedAction;
        public event Action<IShopItemDataBase> ItemTryBuyAction;
        public event Action ItemBuySuccessAction;
        public event Action ItemBuyFailedAction;
        public event Action ContentRenderedEvent;

        private ColorBlock DisabledColorsTab => new ColorBlock() 
        {
            normalColor = Color.clear,
            highlightedColor = buttonTab.colors.highlightedColor,
            pressedColor = buttonTab.colors.pressedColor,
            selectedColor = buttonTab.colors.selectedColor,
            disabledColor = buttonTab.colors.disabledColor,
            colorMultiplier = buttonTab.colors.colorMultiplier,
            fadeDuration = buttonTab.colors.fadeDuration
        };

        private ColorBlock EnabledColorsTab => new ColorBlock()
        {
            normalColor = Color.white,
            highlightedColor = buttonTab.colors.highlightedColor,
            pressedColor = buttonTab.colors.pressedColor,
            selectedColor = buttonTab.colors.selectedColor,
            disabledColor = buttonTab.colors.disabledColor,
            colorMultiplier = buttonTab.colors.colorMultiplier,
            fadeDuration = buttonTab.colors.fadeDuration
        };

        private void OnDestroy() 
        {
            buttonTab.onClick.RemoveListener(() => TabSelectedAction?.Invoke(this));
        }

        public void Initialize() 
        {
            CheckIsHorizontalScroll();
            
            buttonTab.onClick.AddListener(() => TabSelectedAction?.Invoke(this));

            InitializeTab();
        }

        public void OnReset()
        {
            buttonTab.colors = DisabledColorsTab;
        }

        public void MainRender()
        {
            buttonTab.colors = EnabledColorsTab;
            
            SetScrollToLeft();

            if (scroller.content.transform.childCount > 0)
            {
                var needReset = CheckNeedResetContent();

                if (needReset)
                {
                    ResetContent();
                }
                else
                {
                    for (int i = 0; i < scroller.content.transform.childCount; i++) 
                    {
                        scroller.content.transform.GetChild(i).gameObject.Activate();
                    }    
                    return;   
                }
            }
            
            ContentRender();
            
            InjectItems();
            
            ContentRenderedEvent?.Invoke();
            
            Debug.Log("<color=red>Content rendered!</color>");
        }

        private bool CheckNeedResetContent()
        {
            bool needReset = false;

            foreach (var item in Data.ItemsData)
            {
                if (item is ShopGroupItemData groupItem)
                {
                    if (groupItem.items.All(x => x.IsSold))
                    {
                        item.IsSold = true;
                        needReset = true;
                    }

                    if (groupItem.items.FirstOrDefault(x => x.IsSold))
                    {
                        needReset = true;
                    }
                }
                else if (item is ShopItemPremiumCharacterPackData packItem)
                {
                    if (packItem.groupItem.items.All(x => x.IsSold))
                    {
                        packItem.groupItem.IsSold = true;
                        needReset = true;
                    }
                    else if (packItem.groupItem.items.FirstOrDefault(x => x.IsSold))
                        needReset = true;
                }
                else if (scroller.content.transform.childCount != Data.ItemsData.Count(x => x.IsSold == false))
                    needReset = true;
            }

            return needReset;
        }

        protected virtual void InitializeTab() { }
        protected abstract void ContentRender();
        protected abstract void UnRegisterContent();
        
        protected void InvokeItemTryBuyEvent(IShopItemDataBase itemData) => ItemTryBuyAction?.Invoke(itemData);
        protected void InvokeItemBuyFailedEvent() => ItemBuyFailedAction?.Invoke();
        protected void InvokeItemBuySuccessEvent() => ItemBuySuccessAction?.Invoke();
        protected void InvokeItemTryBuyInvokedEvent() => ItemTryBuyInvokedAction?.Invoke();

        private void ResetContent()
        {
            for (int i = 0; i < scroller.content.transform.childCount; i++) 
            {
                GameObject item = scroller.content.transform.GetChild(i).gameObject;
                UnRegisterContent();
                
                if (CurrentItems.Count > 0) CurrentItems.Clear();
                
                Destroy(item);
            }
        }

        private void InjectItems()
        {
            for (int i = 0; i < scroller.content.transform.childCount; i++) 
            {
                GameObject item = scroller.content.transform.GetChild(i).gameObject;
                _diContainer.Inject(item.GetComponent<IShopItemView>());
            }
        }

        private void SetScrollToLeft() => scroller.horizontalNormalizedPosition = 0;

        private void CheckIsHorizontalScroll()
        {
            if (Data.isHorizontalScroll)
            {
                scroller.horizontal = true;
                scroller.vertical = false;
            }
            else
            {
                scroller.horizontal = false;
                scroller.vertical = true;
            }
        }
    }
}