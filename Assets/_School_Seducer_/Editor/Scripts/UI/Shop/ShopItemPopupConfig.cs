using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(fileName = "Popup config", menuName = "Game/Shop/Other/Popup config", order = 0)]
    public class ShopItemPopupConfig : ScriptableObject
    {
        [SerializeField, SerializeReference] public ShopItemPopupViewBase mainPopupView;
        [SerializeField, SerializeReference] public ShopItemPopupDataBase[] data;
    }

    [Serializable]
    public abstract class ShopItemPopupDataBase
    {
        [SerializeReference] public ShopItemPopupViewBase popupView;
        
        public abstract IShopItemDataBase Data { get; }
    }

    [Serializable]
    public class ShopItemPopupData : ShopItemPopupDataBase
    {
        [SerializeField, SerializeReference] public ShopTypeItemDataBase data;

        public override IShopItemDataBase Data => data;
    }

    [Serializable]
    public class ShopSingleGroupItemPopupData : ShopItemPopupDataBase
    {
        [SerializeField, SerializeReference] public ShopSingleItemGroupDataBase data;

        public override IShopItemDataBase Data => data;
    }
}