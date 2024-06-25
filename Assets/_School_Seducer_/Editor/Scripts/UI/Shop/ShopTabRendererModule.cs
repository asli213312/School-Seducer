using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Services;
using _School_Seducer_.Editor.Scripts.Chat;
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
        [SerializeField] private UnityEvent contentRenderedEvent;
        [SerializeField] private UnityEvent itemTryBuyEvent;
        [SerializeField] private UnityEvent itemBuySuccessEvent;
        [SerializeField] private UnityEvent itemBuyFailedEvent;

        private ShopSystem _system;
        private ShopTabViewBase _currentTabView;

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
        }

        private void Start()  
        {
            OnSwitchTab(startTab);

            Invoke("InvokeLoadShopData", 3.5f);
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

        private void InvokeDelegateContentRendered() => contentRenderedEvent?.Invoke(); 

        private void OnSwitchTab(ShopTabViewBase tabView) 
        {
            if (_currentTabView == null) 
            {
                _currentTabView = tabView;

                _currentTabView.transform.GetChild(0).gameObject.Activate();
            } 

            if (_currentTabView == tabView) 
            {
                Debug.LogWarning("Selected current tab!");
            }
            else 
            {
                _currentTabView.transform.GetChild(0).gameObject.Deactivate();
                _currentTabView.OnReset();
                
                _currentTabView = tabView;
                RenderCurrentTab();
                _currentTabView.transform.GetChild(0).gameObject.Activate();
            }
        }

        private void RenderCurrentTab() => _currentTabView.MainRender();
    }
}