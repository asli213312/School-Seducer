using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    public class PlayerConfig : ScriptableObject
    {
        [Header("Player Parameters")] 
        [SerializeField] private int level;
        
        [Header("Money Parameters")]
        [SerializeField] private int money;
        [SerializeField] private int costChooseOption;
        [SerializeField] private int addMoneyAtClick;

        public int Level {get  => level; set => level = value; }
        public int Money {get => money; set => money = value; }
        public int CostChooseOption => costChooseOption;
        public int AddMoneyAtClick => addMoneyAtClick;
    }
}