using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopItemViewCharacter : ShopSingleItemViewBase
    {
        [SerializeField] private Image characterPortrait;
        [SerializeField] private TextMeshProUGUI costText;
        
        protected override IShopItemDataBase Data {get => data; set => data = value as ShopSingleItemDataCharacter; }
        protected ShopSingleItemDataCharacter data;

        protected override void MainRender()
        {
            
            if (data.cost > 0)
                costText.text = "" + data.cost;
            else
                costText.text = "Free";
            characterPortrait.sprite = data.characterData.info.portrait;
            
            RenderAdditional();
        }

        protected virtual void RenderAdditional() { }
    }
}