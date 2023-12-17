using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public interface IСonversation
    {
        Sprite StoryTellerSprite { get; set; }
        Sprite ActorLeftSprite { get; set; }
        Sprite ActorRightSprite { get; set; }
        public ChatConfig Config { get; set; }
    }
}