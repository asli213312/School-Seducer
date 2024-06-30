using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopItemViewCharacterUnlock : ShopItemViewCharacter
    {
        protected override bool TryBuy()
        {
            Info.characterData.isLocked = false;
            return true;
        }
    }
}