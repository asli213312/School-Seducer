using NaughtyAttributes;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    public class PlayerConfig : ScriptableObject
    {
        [Header("Player Parameters")] 
        [SerializeField] private int level;

        [Header("Currencies")] 
        [SerializeField] private int diamonds;
        [SerializeField] private int money;
        
        [Header("Options")]
        [SerializeField] private bool showParameters;
        [SerializeField] private bool showDebugParameters;
        
        [Header("Money Parameters")]
        [SerializeField, ShowIf("showParameters")] private int costNextNode;
        [SerializeField, ShowIf("showParameters")] private int addMoneyAtClick;

        public int Level {get  => level; set => level = value; }
        public int Money {get => money; set => money = value; }
        public int Diamonds {get => diamonds; set => diamonds = value; }
        public int CostNextNode => costNextNode;
        public int AddMoneyAtClick => addMoneyAtClick;
    }
}