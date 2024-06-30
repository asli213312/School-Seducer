using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSingleItemGroupViewComics : ShopSingleItemGroupViewBase
    {
        [SerializeField] private Image ownerCharacter;

        protected override IShopItemDataBase Data {get => _info; set => _info = value as ShopSingleItemGroupComicsInfo; }

        private ShopSingleItemGroupComicsInfo _info;

        public override void AdditionalRender()
        {
            ownerCharacter.sprite = _info.CharacterData.info.onLocationSprite;
        }
        
        protected override bool TryBuy()
        {
            foreach (var gallerySlot in _info.gallerySlotsToUnlock)
            {
                gallerySlot.AddedInGallery = true;
            }

            return true;
        }
    }
}