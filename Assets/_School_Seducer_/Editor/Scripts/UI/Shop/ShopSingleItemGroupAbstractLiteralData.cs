using Sirenix.OdinInspector;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopSingleItemAbstractLiteralData : ShopSingleItemGroupDataBase, IShopItemUnlimitable
    {
        public int value;
        public bool useTimer;
        [ShowIf(nameof(useTimer))] public int timeToWait;

        [Header("Debug")]
        [ShowIf(nameof(showDebugParameters))] public int currentAwaitedTime;
        
        public void LockContent() => currentAwaitedTime = 0;
    }
}