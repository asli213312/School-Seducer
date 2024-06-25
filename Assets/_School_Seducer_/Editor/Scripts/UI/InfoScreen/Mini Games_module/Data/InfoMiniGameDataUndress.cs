using System;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts
{
	[CreateAssetMenu(fileName = "Undress", menuName = "Game/Data/InfoScreen/MiniGames/Undress", order = 0)]
    public class InfoMiniGameDataUndress : InfoMiniGameDataBase
    {
        [SerializeField] public Sprite finalCharacterImage;
        [SerializeField] public MiniGameUndressPointType[] pointsTypes;
        [SerializeField] public InfoMiniGameDataUndressLevel[] levels;
    }

    [Serializable]
    public class InfoMiniGameDataUndressLevel
    {
        public InfoMiniGameDataUndressPoint[] pointsToTouch;
        public Sprite nextImage;
    }

    [Serializable]
    public class InfoMiniGameDataUndressPoint
    {
        public Sprite icon;
        public MiniGameUndressPointType type;
    }

    public enum MiniGameUndressPointType
    {
        Lips, Jaw, Tongue, Hand
    }
}