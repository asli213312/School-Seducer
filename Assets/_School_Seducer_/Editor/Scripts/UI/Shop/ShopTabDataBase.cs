using Sirenix.Utilities;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopTabDataBase : ScriptableObject
    {
        [SerializeField] public bool isHorizontalScroll;
        public abstract IShopItemPurchasable[] ItemsData { get; }
    }
}