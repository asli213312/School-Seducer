using System;
using System.Collections;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSingleItemGroupViewCrystalLiteral : ShopSingleItemGroupViewAbstractLiteral, IShopItemSoldableHardItem
    {
        protected override ShopSingleItemAbstractLiteralData LiteralData { get => _data; set => _data = value as ShopSingleItemGroupCrystalsData; }
        private ShopSingleItemGroupCrystalsData _data;

        protected override bool TryBuy()
        {
            InvokeSoldItem(_data);
            StartCoroutine(Process());
            return false;
        }

        private IEnumerator Process() 
        {
            while (PlayFabManager.IsPositivePayment == false) 
            {
                yield return null;
            }

            InvokeSoldSuccessItem();

            Bank.ChangeValueDiamonds(_data.value);
            PlayFabManager.IsPositivePayment = false;
        }
    }
}