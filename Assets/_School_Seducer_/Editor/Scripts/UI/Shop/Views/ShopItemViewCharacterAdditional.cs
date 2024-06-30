namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopItemViewCharacterAdditional : ShopItemViewCharacter
    {
        protected override bool TryBuy()
        {
            Info.characterData.allConversations.ForEach(x => { x.isUnlocked = true; x.isSeen = false; } );
            return true;
        }
    }
}