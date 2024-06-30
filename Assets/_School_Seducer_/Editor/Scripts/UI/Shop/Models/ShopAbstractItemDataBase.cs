using Sirenix.OdinInspector;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopTypeItemDataBase : ScriptableObject, IShopItemDataBase, IShopItemPurchasable, IShopItemLockable
    {
        [Header("Main data")]
        [SerializeField] public bool isSold;
        [Space(10)]

        [Header("Debug")] 
        [SerializeField] private bool showDebugParameters;
        [SerializeField, ShowIf(nameof(showDebugParameters))] private string id;
        public string Id => id;
        public bool IsSold {get => isSold; set => isSold = value;}
        public IShopItemView View => itemView;
        public string ItemName => name;
        protected abstract IShopItemView itemView { get; }
        
        [ContextMenu("Lock Content")]
        public abstract void LockContent();
    }
    
    public abstract class ShopSingleItemDataBase : ShopTypeItemDataBase, IShopItemCostable
    {
        public float cost;
        [SerializeField, SerializeReference] public ShopSingleItemViewBase view;

        public float Cost => cost;
        public bool NeedSubtract { get; set; } = true;

        protected override IShopItemView itemView => view;
    }
    
    public abstract class ShopSingleItemGroupDataBase : ScriptableObject, IShopItemDataBase, IShopItemPurchasable, IShopItemCostable
    {
        [Header("Main data")] 
        [SerializeField] public bool isSold;
        [SerializeField] public float cost;
        [SerializeField, SerializeReference] public ShopSingleItemGroupViewBase view;
        [Space(10)] 
        
        [Header("Main Debug")] 
        [SerializeField] protected bool showDebugParameters;
        [SerializeField, ShowIf(nameof(showDebugParameters))] private string id;

        public string Id => id;
        public IShopItemView View => view;
        public float Cost => cost;
        public bool NeedSubtract { get; set; } = true;
        public bool IsSold {get => isSold; set => isSold = value;}
        public string ItemName => name;
    }
}