using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSingleItemGroupViewGoldLiteral : ShopSingleItemGroupViewAbstractLiteral
    {
        protected override ShopSingleItemAbstractLiteralData LiteralData { get => _data; set => _data = value as ShopSingleItemGroupGoldData; }
        private ShopSingleItemGroupGoldData _data;
        
        protected override bool TryBuy()
        {
            IShopItemCostable costableItem = _data as IShopItemCostable;

            if (Bank.Diamonds < costableItem.Cost) return false;

            Bank.ChangeValueDiamonds(-Mathf.RoundToInt(costableItem.Cost));
            Bank.ChangeValueGold(_data.value);
            InvokeSoldItem(_data);
            return false;
        }
    }
}