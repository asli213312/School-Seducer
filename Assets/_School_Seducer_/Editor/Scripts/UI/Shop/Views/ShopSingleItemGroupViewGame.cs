using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopSingleItemGroupViewGame : ShopSingleItemGroupViewBase
    {
        [SerializeField] private Image ownerCharacter;

        protected override IShopItemDataBase Data {get => data; set => data = value as ShopSingleItemGroupGameData; }
        private ShopSingleItemGroupGameData data;

        public override void AdditionalRender()
        {
            ownerCharacter.sprite = data.CharacterData.info.onLocationSprite;
        }

        protected override bool TryBuy()
        {
            data.gameData.AddedInGallery = true;
            return true;
        }
    }
}