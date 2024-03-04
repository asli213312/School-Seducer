using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoScreenSystem : MonoBehaviour
    {
        [SerializeField] private Previewer previewer;
        [SerializeField, SerializeReference] private InfoCharacterBaseModule infoModule;
        [SerializeField, SerializeReference] private InfoScrollersModule scrollersModule;
        [SerializeField] private InfoStoryCounter storyCounter;
        [SerializeField] private InfoGiftsModule giftsModule;
        
        public Previewer Previewer => previewer;
        public IInfoCharacterModule InfoModule => infoModule;
        public IInfoScrollersModule ScrollersModule => scrollersModule;
        public InfoStoryCounter StoryCounter => storyCounter;
        public InfoGiftsModule GiftsModule => giftsModule;

        private void Awake()
        {
            InitializeModules();

            void InitializeModules()
            {
                infoModule.InitializeCore(this);
                infoModule.Initialize();
                
                scrollersModule.InitializeCore(this);
                scrollersModule.Initialize();
                
                storyCounter.InitializeCore(this);
                storyCounter.Initialize();
                
                giftsModule.InitializeCore(this);
                giftsModule.Initialize();
            }
        }
    }
}