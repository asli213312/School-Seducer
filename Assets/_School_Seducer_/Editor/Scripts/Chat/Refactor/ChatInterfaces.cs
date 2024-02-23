using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Chat.Refactor;
using _School_Seducer_.Editor.Scripts.Utility;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public interface IModule<in TSystem> where TSystem : MonoBehaviour
    {
        void InitializeCore(TSystem system);
    }

    public interface IChatSystemPlugin : IModule<ChatPlugins>
    {
        
    }

    public interface IChatMessageProcessorModule : IModule<ChatSystem>
    {
        СonversationData CurrentConversation { get; }
        void StartProcessMessages(MessageData[] messages = null);
        GameObject GetUtilityObject();
    }

    public interface IChatStateHandlerModule : IModule<ChatSystem>
    {
        bool IsMessagesEnded { get; set; }
        void StartConversation(Action entryAction = null);
        void InvokeStartConversation();
        void LoadMessages(MessageData[] messages = null);
        void TryEndConversation(bool condition);
        void EndConversation();
        void ResetEndConversation();
        bool CheckEndConversation();
        void ResetContent();
    }
    
    public interface IChatUIManagerModule : IModule<ChatSystem>
    {
        void SetContent();
        void SetContentByMessageData(MessageData messageData);
    }

    public interface IChatInitialization : IModule<ChatSystem>
    {
        Character CurrentCharacter { get; set; }
        void InstallCharacter(Character character);
        void InitializeChats();
    }

    public interface IChatMessageRendererModule : IModule<ChatSystem>
    {
        void RenderMessage(IMessageProxy message, MessageData data, Transform content);
        IMessageProxy InstallPrefabMsg(MessageData messageData);
    }

    public interface IChatStoryResolverModule : IModule<ChatSystem>
    {
        void Initialize();
        void UpdateView();
    }

    public interface IChatTranslationModule : IModule<ChatSystem>, IObservableCustom<MonoBehaviour>
    {
        void UpdateMessages(List<MessageData> messages);
        void TranslateDefaultMessages(List<MessageDefaultViewProxy> messages);
        void TranslateAudioClips();
    }

    public interface IChatSoundHandler
    {
        
    }
}