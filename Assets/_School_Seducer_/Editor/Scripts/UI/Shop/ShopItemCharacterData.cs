using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [Serializable]
    public class ShopItemPremiumCharacterPackData : IShopItemDataBase, IShopItemPurchasable
    {
        [SerializeField] private string itemName;
        [SerializeField, SerializeReference] public ShopSingleItemDataCharacter characterItem;
        [SerializeField, SerializeReference] public ShopGroupItemData groupItem;
        [SerializeField, HideInInspector] private bool isSold;
        [Space(10)]
        
        [Header("Debug")]
        [SerializeField] private bool showDebugParameters;
        [SerializeField, ShowIf("showDebugParameters")] private string id;
        public string Id => id;
        public string ItemName => itemName;
        public bool IsSold { get => isSold; set => isSold = value; }
    }
}