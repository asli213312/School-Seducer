using _School_Seducer_.Editor.Scripts.Chat.Refactor;
using _School_Seducer_.Editor.Scripts.Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [RequireComponent(typeof(IChatInitialization))]
    [ZenjectAllowDuringValidation]
    public sealed class ChatSystem : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private ChatConfig chatConfig;
        
        [Header("Components")]
        [SerializeField] private Transform contentMsgs;
        
        [Header("Modules")] 
        [SerializeReference] private ChatInitializerBase chatInitializerModule;
        [SerializeField] private ChatMessageRenderer chatMessageRendererModule;
        [SerializeField, SerializeReference] private ChatMessageProcessorBase chatMessageProcessorModule;
        [SerializeField] private ChatStateHandler chatStateHandlerModule;
        [SerializeField] private ChatUIManager chatUIManagerModule;
        [SerializeField] private ChatStoryResolver chatStoryResolver;
        [SerializeField] private ChatTranslator chatTranslatorModule;

        public IChatInitialization Initializator => chatInitializerModule;
        public IChatMessageRendererModule MessageRenderer => chatMessageRendererModule;
        public IChatMessageProcessorModule MessageProcessor => chatMessageProcessorModule;
        public IChatStateHandlerModule StateHandler => chatStateHandlerModule;
        public IChatUIManagerModule UIManager => chatUIManagerModule != null ? chatUIManagerModule : null;
        public IChatStoryResolverModule StoryResolver => chatStoryResolver;
        public IChatTranslationModule Translator => chatTranslatorModule;

        public ChatConfig ChatConfig => chatConfig;
        public Transform ContentMsg => contentMsgs;

        private void Awake()
        {
            InitializeModules();

            void InitializeModules()
            {
                chatInitializerModule.InitializeCore(this);
                chatMessageProcessorModule.InitializeCore(this);
                chatStateHandlerModule.InitializeCore(this);
                chatMessageRendererModule.InitializeCore(this);
                chatStoryResolver.InitializeCore(this);
                
                chatUIManagerModule.IsNullReturn();
                chatUIManagerModule.InitializeCore(this);
            }
        }
    }
}