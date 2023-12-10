using NaughtyAttributes;
using UnityEngine;
using HideIfAttribute = Sirenix.OdinInspector.HideIfAttribute;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    [CreateAssetMenu(fileName = "WheelSlotData", menuName = "Game/Data/Wheel Fortune/Slot Data", order = 0)]
    public class WheelSlotData : ScriptableObject
    {
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
        
        [HideIf("useTest")]
        [Range(1, 5)] public int level;
        
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
        [Sirenix.OdinInspector.ShowIf("useTest"), Range(1, 5)] public int levelTest;
        [Sirenix.OdinInspector.ShowIf("useTest"), Sirenix.OdinInspector.MinMaxSlider(1, 100)] public Vector2Int chanceToGainTest;
    }
}