using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;

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
        

        [Header("Options")] 
        public float MainHeight;
        public float OneOptionWidth;
        public float TwoOptionsWidth;
        public float ThreeOptionsWidth;
        
        [Header("Messages")]
        public float DelayBtwMessage;
        public float DurationSendingPicture;
    }
}