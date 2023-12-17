using _School_Seducer_.Editor.Scripts.Chat;
using _School_Seducer_.Editor.Scripts.UI.Gallery;
using UnityEngine;
using NaughtyAttributes;
using UltEvents;

namespace _School_Seducer_.Editor.Scripts
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Game/Data/Character Data", order = 0)]
    public class CharacterData : ScriptableObject
    {
        [Header("Data")]
        [SerializeField] public СonversationData conversation;
        [SerializeField] public GalleryCharacterData gallery;
        
        [Header("Parameters")]
        [SerializeField] private int requiredLevel;
        [SerializeField, MaxValue(10)] private int loyalty = 2;
        
        [Header("Restrictions")]
        [SerializeField, Range(1, 10)] private int maxLoyalty;
        [SerializeField] private bool showDebugParameters;

        private void OnValidate()
        {
            if (loyalty < 1)
                loyalty = 1;
            if (loyalty > maxLoyalty)
                loyalty = maxLoyalty;
        }

        public void ChangeLoyalty(int n)
        {
            if (loyalty + n >= 1 && loyalty + n <= maxLoyalty)
                loyalty += n;
        }

        public int RequiredLevel => requiredLevel;
        public int Loyalty => loyalty;
    }
}