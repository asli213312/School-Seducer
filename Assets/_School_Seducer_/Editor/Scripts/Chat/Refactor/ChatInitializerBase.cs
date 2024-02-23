using _School_Seducer_.Editor.Scripts.UI;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public abstract class ChatInitializerBase : MonoBehaviour, IChatInitialization
    {
        public Character CurrentCharacter { get; set; }
        public СonversationData CurrentConversation { get; private set; }

        private ChatSystem _chatSystem;

        public void InitializeCore(ChatSystem chatSystem)
        {
            _chatSystem = chatSystem;            
        }

        public void InstallCharacter(Character character)
        {
            CurrentCharacter = character;
            CurrentConversation = character.currentConversation;
            
            _chatSystem.StoryResolver.Initialize();
        }

        public void InitializeChats()
        {
            
        }
    }
}