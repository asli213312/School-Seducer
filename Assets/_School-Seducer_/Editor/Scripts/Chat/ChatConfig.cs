using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [CreateAssetMenu]
    public class ChatConfig : ScriptableObject
    {
        [SerializeField] public UnityEvent OnMessageReceived;
        [SerializeField] public UnityEvent OnConversationStart;
        [SerializeField] public UnityEvent OnConversationEnd;

        [Header("Data")] 
        [ShowAssetPreview(32, 32)] public Sprite StoryTellerSprite;
        public string StoryTellerName;
        public bool NeedIconStoryTeller;
        [ShowAssetPreview(32, 32)] public Sprite ChatCompletedSprite;
        [ShowAssetPreview(32, 32)] public Sprite ChatUncompletedSprite;
        public СonversationData[] Chats;
        

        [Header("Options")] 
        public float MainHeight;
        public float OneOptionWidth;
        public float TwoOptionsWidth;
        public float ThreeOptionsWidth;
        
        [Header("Messages")]
        public float DelayBtwMessage;
        public float DurationSendingPicture;
        [Range(1, 10)] public int CoinsForMessage;
    }
}