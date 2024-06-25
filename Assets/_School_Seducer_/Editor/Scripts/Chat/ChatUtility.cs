using _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [AddComponentMenu("Chat Utility")]
    public class ChatUtility : MonoBehaviour
    {
        [SerializeField] private Chat chat;
        [SerializeField] private WheelSlotData[] gifts;
        
        public void LockConversations(CharacterData characterData) => characterData.allConversations.ForEach(x => x.isUnlocked = false);
        public void UnlockConversation(СonversationData conversation) => conversation.isUnlocked = true;
        public void UncompleteConversation(СonversationData conversation) => conversation.isCompleted = false;
        public void ResetMessagesEnded() => chat.ResetConversationComplete();

        public void SetGifts(CharacterData characterData)
        {
            characterData.gifts.Clear();
            
            for (int i = 0; i < 10; i++)
            {
                characterData.gifts.Add(gifts[Random.Range(0, gifts.Length)]);
            }
        }
        public void ResetConversations(CharacterData characterData) => characterData.allConversations.ForEach(x => x.ResetMessages());
        public void ResetConversation(СonversationData conversation) => conversation.ResetMessages();
    }
}