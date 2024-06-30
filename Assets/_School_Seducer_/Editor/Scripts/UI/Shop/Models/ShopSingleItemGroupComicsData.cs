using System;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/Group/Comics", fileName = "Comics")]
    public class ShopSingleItemGroupComicsInfo : ShopSingleItemGroupDataBase, IShopItemLockable, IShopItemCharacterData
    {
        [SerializeField, SerializeReference] public GallerySlotDataBase[] gallerySlotsToUnlock;
        [SerializeField] private string description;
        public CharacterData characterInfo;

        public CharacterData CharacterData => characterInfo;
        public string Description => description;

        public void LockContent()
        {
            foreach (var slot in gallerySlotsToUnlock)
            {
                slot.AddedInGallery = false;
            }
        }
    }
}