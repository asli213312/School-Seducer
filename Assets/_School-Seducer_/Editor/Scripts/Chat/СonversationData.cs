using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Extensions;
using NaughtyAttributes;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [CreateAssetMenu]
    public class СonversationData : ConversationBase, IСonversation
    {
        [field: SerializeField] public ChatConfig Config { get; set; }

        public string ActorLeftName;
        [field: ShowAssetPreview(), SerializeField] public Sprite ActorLeftSprite { get; set; }
        [Space(3)]
        
        public string ActorRightName;
        [field: ShowAssetPreview(), SerializeField] public Sprite ActorRightSprite { get; set; }

        [SerializeField, ShowIf("IsFalse")] public MessageData[] Messages;

        private bool _isFalse;

        private bool IsFalse() => _isFalse = true;

        private void OnValidate()
        {
            for (int i = 0; i < Messages.Length; i++)
            {
                if (Messages[i].optionalData.Branches.Length > 0)
                {
                    MessageData[] newArray = new MessageData[i + 1];
                    Array.Copy(Messages, newArray, i + 1);
                    
                    Array.Resize(ref Messages, i + 1);
                    Array.Copy(newArray, Messages, i + 1);
                    Debug.LogWarning("Messages after installed branches were removed");
                }
            }
            _isFalse = true;
        }
    }
}