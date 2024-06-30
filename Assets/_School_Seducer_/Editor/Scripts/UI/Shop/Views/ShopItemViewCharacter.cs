using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopItemViewCharacter : ShopSingleItemViewBase
    {
        [SerializeField] private Image characterPortrait;
        [SerializeField] private TextMeshProUGUI costText;
        
        protected override IShopItemDataBase Data {get => Info; set => Info = value as ShopSingleItemInfoCharacter; }
        protected ShopSingleItemInfoCharacter Info;

        protected override void MainRender()
        {
            if (Info.cost > 0)
                costText.text = "" + Info.cost;
            else
                costText.text = "Free";
            characterPortrait.sprite = Info.characterData.info.portrait;
            
            RenderAdditional();
        }

        protected virtual void RenderAdditional() { }
    }
}