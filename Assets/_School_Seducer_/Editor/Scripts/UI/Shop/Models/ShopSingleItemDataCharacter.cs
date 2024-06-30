using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts.UI.Shop
{
    public abstract class ShopSingleItemInfoCharacter : ShopSingleItemDataBase, IShopItemCharacterData
    {
        [FormerlySerializedAs("characterInfo")] public CharacterData characterData;
        [SerializeField] private string description;
        public CharacterData CharacterData => characterData;
        public string Description => description;
    }
}