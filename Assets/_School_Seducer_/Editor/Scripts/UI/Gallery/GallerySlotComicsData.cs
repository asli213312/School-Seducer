using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI
{
    [CreateAssetMenu(fileName = "Comics", menuName = "Game/Data/Gallery/SlotComics", order = 0)]
    public class GallerySlotComicsData : GallerySlotDataSpecial
    {
        [SerializeField] public Sprite comics;
    }
}