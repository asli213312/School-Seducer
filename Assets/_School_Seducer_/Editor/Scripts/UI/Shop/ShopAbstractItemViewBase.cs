using System;
using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopTypeItemViewBase : MonoBehaviour, IShopItemView, IShopItemSoldableSoftItem
    {
        [Inject] protected Bank Bank;

        public event Action<IShopItemDataBase> SoldAction;
        public event Action SoldSuccessAction;
        public event Action SoldFailedAction;

        public IShopItemDataBase MainData => Data;

        protected abstract IShopItemDataBase Data { get; set; }

        public void BaseRender(IShopItemDataBase itemData)
        {
            Data = itemData;
            
            MainRender();
        }

        protected virtual void MainRender() { }

        protected abstract bool TryBuy();
        protected void Buy()
        {
            bool isPurchased = TryBuy();

            if (this is ShopSingleItemGroupViewAbstractLiteral) return;

            if (isPurchased)
            {
                IShopItemPurchasable purchasableItem = Data as IShopItemPurchasable;

                if (Data is IShopItemCostable costableItem) 
                {
                    if (Bank.Diamonds < costableItem.Cost) return;

                    Bank.ChangeValueDiamonds(-Mathf.RoundToInt(costableItem.Cost));
                }

                purchasableItem.IsSold = true;
                SoldSuccessAction?.Invoke();
                SoldAction?.Invoke(Data);
                Debug.Log("<color=green>PURCHASED</color>: " + Data.ItemName + " + id: " + Data.Id);
            }

            IShopItemPurchasable purchasableItemm = Data as IShopItemPurchasable;
            Debug.Log("Was try buy item: " + Data.ItemName + " = " + purchasableItemm.IsSold);
        }

        protected void InvokeSoldItem(IShopItemDataBase itemData) => SoldAction?.Invoke(itemData);
        protected void InvokeSoldSuccessItem() => SoldSuccessAction?.Invoke();
    }

    public abstract class ShopSingleItemViewBase : ShopTypeItemViewBase
    {
        [SerializeField] private Button buyButton;

        private void Awake()
        {
            buyButton.AddListener(Buy);
        }

        private void OnDestroy()
        {
            buyButton.RemoveListener(Buy);
        }
    }

    public abstract class ShopGroupItemViewBase : ShopTypeItemViewBase
    {
        [SerializeField] private Button buyButton;
        [SerializeField] protected GridLayoutGroup gridLayout;
        [SerializeField] protected Transform content;
        [SerializeField] protected RectTransform contentScaleableWrapper;
        [SerializeField] protected float offsetLayout = 28.82f;
        protected LayoutElement LayoutElement => GetComponent<LayoutElement>();
        public List<ShopSingleItemGroupViewBase> Items = new();

        protected Action<IShopItemDataBase> SingleItemSoldAction;

        public void InitializeSoldAction(Action<IShopItemDataBase> soldAction) 
        {
            SingleItemSoldAction = soldAction;

            foreach (var item in Items) 
            {
                //item.SoldAction += SingleItemSoldAction;
            }
        }

        protected void InitializeItems()
        {
            foreach (var item in Items)
            {
                item.Initialize(Bank);
                //item.SoldAction += InvokeSoldItem;
            }
        }

        protected void UpdateLayout() 
        {
            LayoutElement.minWidth = contentScaleableWrapper.sizeDelta.x + offsetLayout;
            buyButton.GetComponent<RectTransform>().position = contentScaleableWrapper.position;
            buyButton.GetComponent<RectTransform>().sizeDelta = contentScaleableWrapper.sizeDelta;
        }
    }

    public abstract class ShopSingleItemGroupViewBase : ShopTypeItemViewBase
    {
        [SerializeField] protected Button buyButton;
        [SerializeField] protected TextMeshProUGUI costText;

        public IShopItemDataBase SingleData => Data;

        public void Initialize(Bank bank)
        {
            Bank = bank;
        }

        private void Awake()
        {
            buyButton.AddListener(Buy);
        }

        private void OnDestroy()
        {
            buyButton.RemoveListener(Buy);
        }

        protected override void MainRender()
        {
            AdditionalRender();

            ShopSingleItemGroupDataBase data = Data as ShopSingleItemGroupDataBase;

            if (data.cost > 0)
                costText.text = "" + data.cost;
            else
                costText.text = "Free";
        }

        public virtual void AdditionalRender() { }
    }
}