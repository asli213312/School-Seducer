namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopItemViewCharacterPremium : ShopItemViewCharacter
    {
        protected override bool TryBuy()
        {
            Info.characterData.isLocked = false;
            return true;
        }
    }
}