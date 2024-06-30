using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Services;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.Extensions;
using JetBrains.Annotations;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopTabRendererModule : MonoBehaviour, IModule<ShopSystem>
    {
        [Zenject.Inject] private SaveToDB _saver;

        [SerializeField] private PlayFabManager playFabManager;
        [SerializeField, SerializeReference] private ShopTabViewBase[] tabs;
        [SerializeField, SerializeReference] private ShopTabViewBase startTab;

        [Header("Events")]
        [SerializeField] public UnityEvent contentRenderedEvent;
        [SerializeField] private UnityEvent itemTryBuyEvent;
        [SerializeField] private UnityEvent itemBuySuccessEvent;
        [SerializeField] private UnityEvent itemBuyFailedEvent;

        public event Action OnInitialize;
        public event Action<ShopTabViewBase> OnTabChanged; 
        public ShopTabViewBase[] Tabs => tabs;
        
        public ShopTabViewBase CurrentTabView { get; private set; }

        private ShopSystem _system;

        public void InitializeCore(ShopSystem system)
        {
            _system = system;
        }
        
        public void Initialize()
        {
            tabs.ForEach(x => x.TabSelectedAction = OnSwitchTab);
            tabs.ForEach(x => x.ContentRenderedEvent += InvokeDelegateContentRendered);
            tabs.ForEach(x => x.ItemTryBuyAction += playFabManager.GetStoreItems);
            tabs.ForEach(x => x.ItemTryBuyAction += (item) => itemTryBuyEvent?.Invoke());
            tabs.ForEach(x => x.ItemTryBuyInvokedAction += () => _saver.mainData.SaveShopData(x.Data));
            tabs.ForEach(x => x.ItemTryBuyInvokedAction += () => _saver.SAVE());
            tabs.ForEach(x => x.ItemBuySuccessAction += () => itemBuySuccessEvent?.Invoke());
            tabs.ForEach(x => x.ItemBuyFailedAction += () => itemBuyFailedEvent?.Invoke());
            tabs.ForEach(x => x.Initialize());

            this.WaitForSeconds(0.5f, () => OnInitialize?.Invoke());
        }

        private void Start()  
        {
            OnSwitchTab(startTab);

            Invoke(nameof(InvokeLoadShopData), 3.5f);
        }

        private void InvokeLoadShopData() => _saver.mainData.LoadShopData(tabs);

        private void OnDestroy()
        {
            tabs.ForEach(x => x.ContentRenderedEvent -= InvokeDelegateContentRendered);
            tabs.ForEach(x => x.ItemTryBuyAction -= playFabManager.GetStoreItems);
            tabs.ForEach(x => x.ItemTryBuyAction -= (item) => itemTryBuyEvent?.Invoke());
            tabs.ForEach(x => x.ItemTryBuyInvokedAction -= () => _saver.mainData.SaveShopData(x.Data));
            tabs.ForEach(x => x.ItemTryBuyInvokedAction -= () => _saver.SAVE());
            tabs.ForEach(x => x.ItemBuySuccessAction -= () => itemBuySuccessEvent?.Invoke());
            tabs.ForEach(x => x.ItemBuyFailedAction -= () => itemBuyFailedEvent?.Invoke());
        }

        public void SelectTab(ShopTabViewBase tabView) => OnSwitchTab(tabView);

        private void InvokeDelegateContentRendered()
        {
            contentRenderedEvent?.Invoke();
        }

        private void OnSwitchTab(ShopTabViewBase tabView) 
        {
            if (CurrentTabView == null) 
            {
                CurrentTabView = tabView;

                CurrentTabView.transform.GetChild(0).gameObject.Activate();
            } 

            if (CurrentTabView == tabView) 
            {
                Debug.LogWarning("Selected current tab!");
            }
            else 
            {
                CurrentTabView.transform.GetChild(0).gameObject.Deactivate();
                CurrentTabView.OnReset();
                
                CurrentTabView = tabView;
                RenderCurrentTab();
                CurrentTabView.transform.GetChild(0).gameObject.Activate();
            }
            
            OnTabChanged?.Invoke(CurrentTabView);
        }

        private void RenderCurrentTab() => CurrentTabView.MainRender();
    }
}