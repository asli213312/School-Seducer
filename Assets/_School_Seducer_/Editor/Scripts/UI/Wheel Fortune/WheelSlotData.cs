using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using HideIfAttribute = Sirenix.OdinInspector.HideIfAttribute;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    [CreateAssetMenu(fileName = "WheelSlotData", menuName = "Game/Data/Wheel Fortune/Slot Data", order = 0)]
    public class WheelSlotData : ScriptableObjectCounter
    {
        [Header("Info Gifts Data")]
        [HideIf(nameof(IsCharacter))] public Sprite iconInfo;
        [HideIf(nameof(IsCharacter))] public Sprite borderSpriteByColor;

        [FormerlySerializedAs("extraWinCharacter")]
        [Header("Extra")] 
        [HideIf(nameof(IsGift))] public Sprite extraCharacterPortrait;
        
        [Header("General")]

        [HideIf("TestOrIsMoneySlot")]
        [ShowAssetPreview(32, 32)] public Sprite icon;

        private bool TestOrIsMoneySlot()
        {
            return IsMoneySlot(slotType) || useTest;
        }

        [HideIf("useTest")]
        private bool IsMoneySlot(WheelCategorySlotEnum slotType) => slotType == WheelCategorySlotEnum.Money;
        
        [HideIf("useTest")]
        public WheelCategorySlotEnum slotType = WheelCategorySlotEnum.Gift;
        
        [HideIf(nameof(IsCharacter))] public int costExp;
        
        [FormerlySerializedAs("level")] [HideIf("useTest")] [Range(1, 5), HideIf(nameof(IsCharacter))] public int score;
        
        [HideIf("useTest"), Sirenix.OdinInspector.MinMaxSlider(1, 100)]
        public Vector2Int chanceToGain;

        [Header("Test")] 
        public bool useTest;
        
        [HideIf("HideTestOrIsNotMoneySlot")]
        [ShowAssetPreview(32, 32)] public Sprite iconTest;

        private bool HideTestOrIsNotMoneySlot()
        {
            return useTest == false || IsMoneySlot(slotTypeTest);
        }

        [Sirenix.OdinInspector.ShowIf("useTest")] public WheelCategorySlotEnum slotTypeTest = WheelCategorySlotEnum.Gift;
        [Sirenix.OdinInspector.ShowIf(nameof(useTest))] public int costExpTest;
        [Sirenix.OdinInspector.ShowIf("useTest"), Range(1, 5)] public int levelTest;
        [Sirenix.OdinInspector.ShowIf("useTest"), Sirenix.OdinInspector.MinMaxSlider(1, 100)] public Vector2Int chanceToGainTest;

        public Vector2 GetProbabilityRange()
        {
            return IsTest() ? chanceToGainTest : chanceToGain;
        }
        public Sprite GetCurrentIcon() => IsTest() ? iconTest : icon;
        public int GetCostExp() => IsTest() ? costExpTest : costExp;
        public bool IsCharacter() => useTest ? slotTypeTest == WheelCategorySlotEnum.Character : slotType == WheelCategorySlotEnum.Character;
        public bool IsGift() => useTest ? slotTypeTest == WheelCategorySlotEnum.Gift : slotType == WheelCategorySlotEnum.Gift;
        private bool IsTest() => useTest;
    }
}