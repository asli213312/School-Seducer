using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSingleItemGroupViewComics : ShopSingleItemGroupViewBase
    {
        [SerializeField] private Image ownerCharacter;

        protected override IShopItemDataBase Data {get => data; set => data = value as ShopSingleItemGroupComicsData; }

        private ShopSingleItemGroupComicsData data;

        public override void AdditionalRender()
        {
            ownerCharacter.sprite = data.characterInfo.onLocationSprite;
        }
        
        protected override bool TryBuy()
        {
            foreach (var gallerySlot in data.gallerySlotsToUnlock)
            {
                gallerySlot.AddedInGallery = true;
            }

            return true;
        }
    }
}