using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    [CreateAssetMenu(fileName = "BranchData", menuName = "Game/Data/Chat/BranchData", order = 0)]
    public class BranchData : ConversationBase, IСonversation
    {
        [SerializeField] [ResizableTextArea] public string BranchName;
        public AudioClip audioMsg;
        [field: NonSerialized] public ChatConfig Config { get; set; }
        [field: ShowAssetPreview(32, 32), SerializeField] public Sprite StoryTellerSprite { get; set; }
        [field: SerializeField, ShowAssetPreview(32, 32)] public Sprite ActorRightSprite { get; set; }
        [field: SerializeField, ShowAssetPreview(32, 32)] public Sprite ActorLeftSprite { get; set; }
        
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
                        spriteToSet = StoryTellerSprite;
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