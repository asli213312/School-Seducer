using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSystem : MonoBehaviour
    {
        [SerializeField] private ShopTabRendererModule tabRendererModule;

        public ShopTabRendererModule TabRendererModule => tabRendererModule;

        private void Awake()
        {
            tabRendererModule.InitializeCore(this);
            tabRendererModule.Initialize();
        }
    }
}