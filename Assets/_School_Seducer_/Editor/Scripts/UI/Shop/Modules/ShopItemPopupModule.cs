using System;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UniRx;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopItemPopupModule : MonoBehaviour, IModule<ShopSystem>
    {
        [Zenject.Inject] private Bank _bank;
        
        [SerializeField] private ShopItemPopupConfig data;
        [SerializeField] private RectTransform popupContainer;
        
        [Header("Options")] 
        [SerializeField] private float destroyPopupDelay;

        [Header("Events")] 
        [SerializeField] private UnityEvent openPopupEvent;
        [SerializeField] private UnityEvent closePopupEvent;
        [SerializeField] private UnityEvent buyPopupEvent;
        
        private List<ShopTabViewBase> _initializedTabs = new();
        private ShopItemPopupViewBase _currentPopup;
        
        private ShopSystem _system;

        public void InitializeCore(ShopSystem system)
        {
            _system = system;
        }

        public void Initialize()
        {
            _system.TabRendererModule.contentRenderedEvent.AddListener(() =>
            {
                InitializePopupRenderedTab(_system.TabRendererModule.CurrentTabView);
            }); 
        }

        private void OnDestroy()
        {
            _system.TabRendererModule.contentRenderedEvent.RemoveListener(() =>
            {
                InitializePopupRenderedTab(_system.TabRendererModule.CurrentTabView);
            }); 
            UnregisterPopups();
        }

        private void OnBuy(List<IShopItemView> itemViews)
        {
            buyPopupEvent?.Invoke();
            OnClosePopup();
        }

        private void OnClosePopup()
        {
            ShopItemPopupViewBase instancePopup = _currentPopup;
            
            StartCoroutine(instancePopup.transform.DoLocalScale(Vector2.zero, () =>
            {
                instancePopup.OnAfterBuy -= OnBuy;
                instancePopup.OnClose -= () => closePopupEvent?.Invoke();
                instancePopup.OnClose -= OnClosePopup;
                this.WaitForSeconds(destroyPopupDelay, () => instancePopup.gameObject.Destroy());
            }));
            
            _currentPopup = null;
        }

        public void InitializePopupRenderedTab(ShopTabViewBase tab)
        {
            if (tab.CurrentItems.Count == 1) return;
            
            if (_initializedTabs.Contains(tab)) return;

            foreach (var itemView in tab.CurrentItems)
            {
                if (itemView.MainData is ShopSingleItemAbstractLiteralData) continue;

                if (data.data.Any(x => x.Data == itemView.MainData))
                {
                    foreach (var popupConfig in data.data)
                    {
                        if (itemView.MainData == popupConfig.Data)
                        {
                            itemView.OnClick += _ => CreateItemPopupByConfig(popupConfig, itemView);
                            Debug.Log($"Item: {itemView.MainData} will be invoked by CUSTOM popup {popupConfig.popupView}");

                             if (itemView is ShopGroupItemViewBase groupItemView)
	                         {
	                            foreach (var singleGroupItem in groupItemView.Items)
	                            {
	                                if (singleGroupItem.MainData != popupConfig.Data && singleGroupItem.MainData is not IShopItemCharacterData) continue;
	                                singleGroupItem.OnClick += _ => CreateItemPopupByConfig(popupConfig, singleGroupItem);
	                                Debug.Log($"Item: {singleGroupItem.MainData} will be invoked by CUSTOM popup {popupConfig.popupView}");
	                            }
	                         }
                        }
                    }   
                }
                else
                {
                    if (itemView.MainData is IShopItemCharacterData) 
                    {
                       itemView.OnClick += CreateItemDefaultPopup;
                       Debug.Log($"Item: {itemView.MainData} will be invoked by DEFAULT popup");
                    }
                    else if (itemView is ShopGroupItemViewBase groupView) 
                    {
                        foreach (var singleItem in groupView.Items) 
                        {
                           if (singleItem.MainData is IShopItemCharacterData) 
                           {
                               singleItem.OnClick += CreateItemDefaultPopup;
                               Debug.Log($"Item: {singleItem.MainData} will be invoked by DEFAULT popup");
                           }
                        }
                    }
                }
            }
            
            _initializedTabs.Add(tab);
        }

        private void CreateItemPopupByConfig(ShopItemPopupDataBase popupItem, IShopItemView defaultItemView)
        {
        	Debug.Log("SELECTED CUSTOM POPUP in shop");

            ShopPremiumItemPopupView selectedPopupView = popupItem.popupView as ShopPremiumItemPopupView;
            IShopItemDataBase selectedData = defaultItemView.MainData;
            List<IShopItemView> selectedViews = new List<IShopItemView> { defaultItemView };

            foreach (var itemData in _system.TabRendererModule.CurrentTabView.Data.ItemsData)
            {
                if (itemData is not ShopItemPremiumCharacterPackData packData) continue;
                selectedViews.Clear();
                
                foreach (var itemView in _system.TabRendererModule.CurrentTabView.CurrentItems)
                {
                    if (ReferenceEquals(itemView.MainData, packData.groupItem) || ReferenceEquals(itemView.MainData, packData.characterItem))
                        selectedViews.Add(itemView);
                }

                if (popupItem.Data is ShopGroupItemData popupGroupData)
                {
                    if (packData.groupItem == popupGroupData)
                    {
                        selectedData = packData;
                    }
                }
                else if (popupItem.Data is ShopSingleItemInfoCharacter popupCharacterData)
                {
                    if (packData.characterItem == popupCharacterData)
                        selectedData = packData;
                }
            }

            Debug.Log("Selected data to render premium popup: " + selectedData);

            ShopPremiumItemPopupView popupView = Instantiate(selectedPopupView, popupContainer.transform);
            popupView.Initialize(_bank);
            popupView.Render(selectedData, selectedViews);
            popupView.OnClose += () => closePopupEvent?.Invoke();
            popupView.OnClose += OnClosePopup;
            popupView.OnAfterBuy += OnBuy;
            
             _currentPopup = popupView;
             
             StartCoroutine(popupView.transform.DoLocalScale(Vector2.one));
             openPopupEvent?.Invoke();
        }

        private void CreateItemDefaultPopup(IShopItemView defaultItemView)
        {
			Debug.Log("SELECTED DEFAULT POPUP in shop");

            ShopItemPopupViewBase selectedPopupView = data.mainPopupView;
            IShopItemDataBase selectedData = defaultItemView.MainData;
            List<IShopItemView> selectedViews = new List<IShopItemView> { defaultItemView };

            ShopItemPopupViewBase popupView = Instantiate(selectedPopupView, popupContainer.transform);
            popupView.Initialize(_bank);
            popupView.Render(selectedData, selectedViews);
            popupView.OnClose += () => closePopupEvent?.Invoke();
            popupView.OnClose += OnClosePopup;
            popupView.OnAfterBuy += OnBuy;

            _currentPopup = popupView;
            
            StartCoroutine(popupView.transform.DoLocalScale(Vector2.one));
            openPopupEvent?.Invoke();
        }

        private void UnregisterPopups()
        {
            foreach (var tab in _initializedTabs)
            {
                foreach (var itemView in tab.CurrentItems)
                {
                    if (itemView.MainData is ShopSingleItemAbstractLiteralData) continue;

                if (data.data.Any(x => x.Data == itemView.MainData))
                {
                    foreach (var popupConfig in data.data)
                    {
                        if (itemView.MainData == popupConfig.Data)
                        {
                            itemView.OnClick -= _ => CreateItemPopupByConfig(popupConfig, itemView);
                            Debug.Log($"Item: {itemView.MainData} will be invoked by CUSTOM popup {popupConfig.popupView}");

                             if (itemView is ShopGroupItemViewBase groupItemView)
	                         {
	                            foreach (var singleGroupItem in groupItemView.Items)
	                            {
	                                if (singleGroupItem.MainData != popupConfig.Data && singleGroupItem.MainData is not IShopItemCharacterData) continue;
	                                singleGroupItem.OnClick -= _ => CreateItemPopupByConfig(popupConfig, singleGroupItem);
	                                Debug.Log($"Item: {singleGroupItem.MainData} will be invoked by CUSTOM popup {popupConfig.popupView}");
	                            }
	                         }
                        }
                    }   
                }
                else
                {
                    if (itemView.MainData is IShopItemCharacterData) 
                    {
                       itemView.OnClick -= CreateItemDefaultPopup;
                       Debug.Log($"Item: {itemView.MainData} will be invoked by DEFAULT popup");
                    }
                    else if (itemView is ShopGroupItemViewBase groupView) 
                    {
                        foreach (var singleItem in groupView.Items) 
                        {
                           if (singleItem.MainData is IShopItemCharacterData) 
                           {
                               singleItem.OnClick -= CreateItemDefaultPopup;
                               Debug.Log($"Item: {singleItem.MainData} will be invoked by DEFAULT popup");
                           }
                        }
                    }
                }
                }
            }
        }
    }
}