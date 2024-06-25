using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/Group/Game", fileName = "Game")]
    public class ShopSingleItemGroupGameData : ShopSingleItemGroupDataBase, IShopItemLockable
    {
        [SerializeField] public GallerySlotDataGame gameData;
        [SerializeField] public CharacterInfo characterInfo;
        
        public void LockContent()
        {
            if (gameData.AddedInGallery)
                gameData.AddedInGallery = false;
        }
    }
}