using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Utility;
using _School_Seducer_.Editor.Scripts.Utility.Translation;
using NaughtyAttributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UltEvents;
using UnityEngine.Serialization;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.Chat
{
	public class ChatConfig : LocalizedScriptableObject
	{
		[SerializeField] public UnityEvent OnMessageReceived;
		[SerializeField] public UnityEvent OnConversationStart;
        [SerializeField] public UnityEvent OnConversationEnd;

        [Header("Data")] 
        [ShowAssetPreview(32, 32)] public Sprite StoryTellerSprite;
        
		[SerializeField, Sirenix.OdinInspector.ReadOnly] public string storyTellerName;
        public bool NeedIconStoryTeller;
        [ShowAssetPreview(32, 32)] public Sprite ChatCompletedSprite;
        [ShowAssetPreview(32, 32)] public Sprite ChatUncompletedSprite;
        public СonversationData[] Chats;
        
        [Header("Options")]
        [FoldoutGroup("Option Button")] public float MainHeight;
        [FoldoutGroup("Option Button")] public float OneOptionWidth;
        [FoldoutGroup("Option Button")] public float TwoOptionsWidth;
        [FoldoutGroup("Option Button")] public float ThreeOptionsWidth;
        [FoldoutGroup("Option Button")] public float delayAudioMsg;
        
        [Header("Messages")]
        public float DelayBtwMessage;
        public float DurationSendingPicture;
        [Range(1, 10)] public int CoinsForMessage;
	}
}