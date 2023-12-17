using System;
using System.Collections.Generic;
using _School_Seducer_.Editor.Scripts.Extensions;
using _School_Seducer_.Editor.Scripts.UI;
using NaughtyAttributes;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [CreateAssetMenu(fileName = "ConversationData", menuName = "Game/Data/Chat/ConversationData", order = 0)]
    public class СonversationData : ConversationBase, IСonversation
    {
        public bool IsCompleted;
        public int ConversationIndex;
        [field: SerializeField] public ChatConfig Config { get; set; }
        [field: ShowAssetPreview(32, 32), SerializeField] public Sprite StoryTellerSprite { get; set; }

        public string ActorLeftName;
        [field: ShowAssetPreview(32, 32), SerializeField] public Sprite ActorLeftSprite { get; set; }
        [Space(3)]
        
        public string ActorRightName;
        [field: ShowAssetPreview(32, 32), SerializeField] public Sprite ActorRightSprite { get; set; }

        [SerializeField] public MessageData[] Messages;

        private void OnValidate()
        {
            if (ActorLeftSprite == null || ActorRightSprite == null || StoryTellerSprite == null)
            {
                Debug.LogWarning("Set actor sprites in inspector!");
            }
            else
            {
                foreach (var msg in Messages)
                {
                    Sprite spriteToSet;
                    if (msg.ActorIcon == ActorLeftSprite)
                    {
                        spriteToSet = ActorLeftSprite;
                        msg.Sender = MessageSender.ActorLeft;
                    }
                    else if (msg.ActorIcon == ActorRightSprite)
                    {
                        spriteToSet = ActorRightSprite;
                        msg.Sender = MessageSender.ActorRight;
                    }
                    else
                    {
                        spriteToSet = Config.StoryTellerSprite;
                        msg.Sender = MessageSender.StoryTeller;
                    }
                    msg.ActorIcon = spriteToSet;
                }
            }
            
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