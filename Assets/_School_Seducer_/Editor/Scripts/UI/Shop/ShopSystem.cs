using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSystem : MonoBehaviour
    {
        [SerializeField] private ShopTabRendererModule tabRendererModule;
        [SerializeField] private ShopItemPopupModule itemPopupModule;

        public ShopTabRendererModule TabRendererModule => tabRendererModule;
        public ShopItemPopupModule ItemPopupModule => itemPopupModule;

        private void Awake()
        {
            tabRendererModule.InitializeCore(this);
            tabRendererModule.Initialize();
            
            itemPopupModule.InitializeCore(this);
            itemPopupModule.Initialize();
        }
    }
}