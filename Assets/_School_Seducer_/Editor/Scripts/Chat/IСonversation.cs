using _School_Seducer_.Editor.Scripts.Utility.Translation;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.Chat
{
    public interface IСonversation
    {
        LocalizatorMessages Localizator { get; }
        Sprite ActorLeftSprite { get; set; }
        Sprite ActorRightSprite { get; set; }
    }
}