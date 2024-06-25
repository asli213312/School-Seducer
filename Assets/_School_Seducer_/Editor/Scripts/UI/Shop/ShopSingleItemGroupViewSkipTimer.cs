using System;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSingleItemGroupViewSkipTimer : ShopSingleItemGroupViewBase
    {
        [SerializeField] private UnityEvent skipEvent;

        protected override IShopItemDataBase Data {get => data; set => data = value as ShopSingleItemGroupSkipTimerData; }

        private ShopSingleItemGroupSkipTimerData data;
        
        protected override bool TryBuy()
        {
            skipEvent?.Invoke();
            return true;
        }
    }
}