using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Map
{
    public class MapSystem : MonoBehaviour
    {
        [SerializeField] private Previewer previewer;
        [SerializeField] private MapSelectorBase selectorModule;

        public Previewer Previewer => previewer;

        private void Awake()
        {
            selectorModule.InitializeCore(this);
            selectorModule.Initialize();
        }
    }
}