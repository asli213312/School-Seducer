using System;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopItemPopupViewBase : MonoBehaviour
    {
        [Header("Base")] 
        [SerializeField] protected Button buyButton;
        [SerializeField] protected Button closeButton;
        [SerializeField] protected TextMeshProUGUI costText;

        [Header("Additional")] 
        [SerializeField, HideLabel] private float blank;

        public event Action<List<IShopItemView>> OnAfterBuy;
        public event Action OnClose;
        public event Action OnOpen;

        protected Bank Bank;
        protected abstract List<IShopItemView> ItemViews { get; set; }
        protected abstract IShopItemDataBase ItemData { get; }

        protected virtual void Awake()
        {
            buyButton.AddListener(() => OnBuy(ItemViews));
            closeButton.AddListener(Close);
        }

        protected void OnDestroy()
        {
            buyButton.RemoveListener(() => OnBuy(ItemViews));
            closeButton.RemoveListener(Close);
        }

        public virtual void Initialize(Bank bank)
        {
            Bank = bank;
        }

        public void Render<T>(T itemData, List<IShopItemView> itemViews) where T : IShopItemDataBase
        {
            if (IsValidType(itemData))
            {
                OnRender(itemData, itemViews);
            }
        }

        protected abstract bool IsValidType<T>(T itemData) where T : IShopItemDataBase;

        protected virtual void OnRender<T>(T data, List<IShopItemView> shopItemViews) where T : IShopItemDataBase
        {
            IShopItemCostable costableItem = (IShopItemCostable)data;
            costText.text = costableItem.Cost.ToString();
            
            OnOpen?.Invoke();
        }

        protected virtual void OnBuy(List<IShopItemView> itemViews)
        {
            IShopItemCostable costableItem = ItemData as IShopItemCostable;

            if (ItemData is ShopItemPremiumCharacterPackData packData)
            {
                if (Bank.Diamonds < costableItem.Cost) return;
                Bank.ChangeValueDiamonds(-Mathf.RoundToInt(costableItem.Cost));
                
                foreach (var itemInPack in itemViews)
                {
                    if (itemInPack.MainData is IShopItemCostable costableItemPack)
                        costableItemPack.NeedSubtract = false;
                }   
            }

            foreach (var itemView in itemViews)
            {
                itemView.Buy();
            }
            
            OnAfterBuy?.Invoke(itemViews);
        }

        protected virtual void Close()
        {
            OnClose?.Invoke();
        }
    }
}