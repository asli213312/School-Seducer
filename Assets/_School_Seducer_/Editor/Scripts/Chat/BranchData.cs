using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [CreateAssetMenu]
    public class BranchData : ConversationBase, IСonversation
    {
        [SerializeField] [ResizableTextArea] public string BranchName;
        public AudioClip audioMsg;
        
        [field: NonSerialized] public ChatConfig Config { get; set; }

        [field: NonSerialized] public Sprite ActorRightSprite { get; set; }
        [field: NonSerialized] public Sprite ActorLeftSprite { get; set; }
        
        [SerializeField] public MessageData[] Messages;

        private void OnValidate()
        {
            for (int i = 0; i < Messages.Length; i++)
            {
                if (Messages[i].optionalData.Branches.Length > 0 && Messages[i] != Messages[^1])
                {
                    MessageData[] newArray = new MessageData[i + 1];
                    Array.Copy(Messages, newArray, i + 1);
                    
                    Array.Resize(ref Messages, i + 1);
                    Array.Copy(newArray, Messages, i + 1);
                    Debug.LogWarning("Messages after installed branches were removed");
                }
            }
        }
    }
}