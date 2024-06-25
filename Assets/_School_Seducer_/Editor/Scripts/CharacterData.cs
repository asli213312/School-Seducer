using System.Collections.Generic;
using System.Linq;
using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.UI.Gallery;
using _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune;
using Sirenix.OdinInspector;
using UnityEngine;
using UltEvents;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Data/Character Data", order = 0)]
    public class CharacterData : ScriptableObject
    {
        [Header("Data")] 
        [SerializeField] public CharacterInfo info;
        [SerializeField] public СonversationData currentConversation;
        [SerializeField] public List<WheelSlotData> gifts;
        [SerializeField] public List<СonversationData> allConversations;
        [SerializeField] public GalleryCharacterData gallery;

        [Header("Parameters")]
        [SerializeField] private int requiredLevel;
        [SerializeField, MaxValue(10)] private int loyalty = 2;
        
        [Header("Restrictions")]
        [SerializeField, Range(1, 10)] private int maxLoyalty;

        [Header("Debug")]
        [SerializeField] private bool showDebugParameters;
        [SerializeField, ShowIf(nameof(showDebugParameters))] public int experience;
        [SerializeField, ShowIf(nameof(showDebugParameters))] public bool isLocked;

        public СonversationData LockedConversation { get; set; }
        public СonversationData UnSeenConversation { get; set; }

        [ContextMenu("Reset Unlocked Conversations")]
        private void ResetUnlockedConversations() => allConversations.ForEach(x => x.isUnlocked = false);
        
        [ContextMenu("Reset Conversations")]
        private void ResetConversations() => allConversations.ForEach(x => x.ResetMessages());

        private void OnValidate()
        {
            if (loyalty < 1)
                loyalty = 1;
            if (loyalty > maxLoyalty)
                loyalty = maxLoyalty;
        }

        public bool HasUnseenConversations() 
        {
            return allConversations.Any(x => x.isUnlocked && x.isSeen == false);
        }

        public bool CanUnlockStory()
        {
            if (LockedConversation == null) return false;
            
            int currentExp = experience;
            int giftsExp = GetGiftsScore();
            int needExp = LockedConversation.costExp;

            return currentExp + giftsExp >= needExp;

            int GetGiftsScore() 
            {
                int count = 0;

                foreach (var gift in gifts) 
                {
                    count += gift.score;
                }

                return count;
            }
        }

        public void AddGift(WheelSlotData gift, int max)
        {
            if (gift.slotType != WheelCategorySlotEnum.Gift || gift.slotTypeTest != WheelCategorySlotEnum.Gift) return;
            
            if (gifts.Count >= max) return;
            gifts.Add(gift);
        }

        public bool LastConversationAvailable()
        {
            return allConversations[^1].isUnlocked;
        }

        public bool ConversationsUnlocked()
        {
            foreach (var conversation in allConversations)
            {
                if (conversation.isUnlocked == false) return false;
            }

            return true;
        }

        public void ChangeExperience(int value)
        {
            if (experience + value >= 0) experience += value;
        }

        public void ChangeLoyalty(int n)
        {
            if (loyalty + n >= 1 && loyalty + n <= maxLoyalty)
                loyalty += n;
        }

        public int RequiredLevel => requiredLevel;
    }
}