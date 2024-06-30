using UnityEngine;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    [CreateAssetMenu(menuName = "Game/Shop/Character/Additional Content Character", fileName = "Additional Content Character")]
    public class ShopSingleItemInfoCharacterAdditional : ShopSingleItemInfoCharacter
    {
        public override void LockContent()
        {
            characterData.allConversations.ForEach(x => { x.isUnlocked = false; x.isSeen = false; } );
        }
    }
}