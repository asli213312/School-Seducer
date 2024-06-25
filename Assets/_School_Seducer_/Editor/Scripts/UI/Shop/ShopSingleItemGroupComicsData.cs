using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/Group/Comics", fileName = "Comics")]
    public class ShopSingleItemGroupComicsData : ShopSingleItemGroupDataBase, IShopItemLockable
    {
        [SerializeField, SerializeReference] public GallerySlotDataBase[] gallerySlotsToUnlock;
        public CharacterInfo characterInfo;
        
        public void LockContent()
        {
            foreach (var slot in gallerySlotsToUnlock)
            {
                slot.AddedInGallery = false;
            }
        }
    }
}