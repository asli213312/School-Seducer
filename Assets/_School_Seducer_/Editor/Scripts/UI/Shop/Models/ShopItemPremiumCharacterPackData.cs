using UnityEngine;
using System;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [Serializable]
    public class ShopItemPremiumCharacterPackData : IShopItemDataBase, IShopItemPurchasable, IShopItemCostable
    {
        [SerializeField] private string itemName;
        [SerializeField] private float cost;
        [SerializeField, SerializeReference] public ShopSingleItemInfoCharacter characterItem;
        [SerializeField, SerializeReference] public ShopGroupItemData groupItem;
        [SerializeField, Multiline] public string description;
        [SerializeField, HideInInspector] private bool isSold;
        [Space(10)]
        
        [Header("Debug")]
        [SerializeField] private bool showDebugParameters;
        [SerializeField, Sirenix.OdinInspector.ShowIf("showDebugParameters")] private string id;
        public string Id => id;
        public string ItemName => itemName;
        public bool IsSold { get => isSold; set => isSold = value; }
        public float Cost => cost;
        public bool NeedSubtract { get; set; } = true;
    }
}