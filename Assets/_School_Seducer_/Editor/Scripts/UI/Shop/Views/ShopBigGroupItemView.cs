using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public class ShopBigGroupItemView : ShopGroupItemViewBase
    {
        [SerializeField] private RectTransform bigItemEmptyPrefab;
        [SerializeField] private float scaleOffset;
        [SerializeField] private float positionOffsetX = 164.8f;
        [SerializeField] private float positionOffsetY = 162.5f;
        
        protected override IShopItemDataBase Data {get => data; set => data = value as ShopGroupItemData; }
        private ShopGroupItemData data;

        protected override void MainRender()
        {
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

            gridLayout.constraintCount = data.columnsAmount;

            foreach (var itemData in data.items)
            {
                if (itemData.IsSold) continue;
                
                RectTransform emptyItem = Instantiate(bigItemEmptyPrefab, content);

                ShopSingleItemGroupViewBase singleItemGroup = Instantiate(itemData.view, emptyItem.transform);
                
                Items.Add(singleItemGroup);

                singleItemGroup.SoldAction += InvokeSoldItem;
                
                Image itemImage = singleItemGroup.GetComponent<Image>();
                itemImage.enabled = false;
                emptyItem.GetComponent<Image>().sprite = itemImage.sprite;
                singleItemGroup.transform.localScale = new Vector3(scaleOffset, scaleOffset, scaleOffset);
                singleItemGroup.BaseRender(itemData);
                singleItemGroup.AdditionalRender();

                singleItemGroup.GetComponent<RectTransform>().anchoredPosition = new Vector2(emptyItem.anchoredPosition.x + positionOffsetX, Math.Abs(emptyItem.anchoredPosition.y + positionOffsetY));
            }

            Invoke(nameof(UpdateLayout), 0.1f);
            Invoke(nameof(InitializeItems), 0.1f);
        }

        private void OnDestroy() 
        {
            foreach (var item in Items)
            {
                item.SoldAction -= InvokeSoldItem;
            }
        }
    }
}