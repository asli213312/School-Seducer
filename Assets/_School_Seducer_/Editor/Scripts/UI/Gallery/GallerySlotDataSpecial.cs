using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI
{
    public abstract class GallerySlotDataSpecial : GallerySlotDataBase
    {
        [SerializeField] public string label;
        [SerializeField] public Sprite preview;
        [SerializeField] public Sprite frame;
    }
}