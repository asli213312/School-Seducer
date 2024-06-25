namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopItemViewCharacterPremium : ShopItemViewCharacter
    {
        protected override bool TryBuy()
        {
            data.characterData.isLocked = false;
            return true;
        }
    }
}