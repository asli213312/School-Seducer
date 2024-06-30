using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/Character/CharacterPremium", fileName = "Character Premium")]
    public class ShopSingleItemInfoCharacterPremium : ShopSingleItemInfoCharacter
    {
        public override void LockContent()
        {
            characterData.isLocked = true;
        }
    }
}