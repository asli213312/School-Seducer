using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/Group/Game", fileName = "Game")]
    public class ShopSingleItemGroupGameData : ShopSingleItemGroupDataBase, IShopItemLockable, IShopItemCharacterData
    {
        [SerializeField] public GallerySlotDataGame gameData;
        [SerializeField] public CharacterData characterInfo;
        [SerializeField] private string description;
        
        public CharacterData CharacterData => characterInfo;
        public string Description => description;

        public void LockContent()
        {
            if (gameData.AddedInGallery)
                gameData.AddedInGallery = false;
        }
    }
}