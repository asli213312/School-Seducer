using System;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public interface IShopItemSoldable
    {
        public event Action<IShopItemDataBase> SoldAction;
        public event Action SoldFailedAction;
        public event Action SoldSuccessAction;
    }

    public interface IShopItemSoldableSoftItem : IShopItemSoldable {}
    public interface IShopItemSoldableHardItem : IShopItemSoldable {}

    public interface IShopItemDataBase
    {
        public string Id { get; }
        public string ItemName { get; }
    }

    public interface IShopItemUnlimitable {}

    public interface IShopItemLockable
    {
        public void LockContent();
    }

    public interface IShopItemCostable 
    {
        public float Cost { get; }
    }

    public interface IShopItemPurchasable
    {
        public bool IsSold { get; set; }
    }

    public interface IShopItemData : IShopItemDataBase
    {
        public IShopItemView View { get; }
    }

    public interface IShopMultipleItemData : IShopItemDataBase
    {
        
    }

    public interface IShopItemView
    {
        public IShopItemDataBase MainData { get; }
        public void BaseRender(IShopItemDataBase itemData);
    }
}