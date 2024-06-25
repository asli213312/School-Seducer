using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI
{
    [CreateAssetMenu(fileName = "Special Game", menuName = "Game/Data/Gallery/Special SlotGame", order = 0)]
    public class GallerySlotDataGame : GallerySlotDataSpecial
    {
        [SerializeField, SerializeReference] public InfoMiniGameDataBase miniGameData;
    }
}