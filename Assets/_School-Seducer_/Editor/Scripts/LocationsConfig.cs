using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    public class LocationsConfig : ScriptableObject
    {
        [SerializeField] private Sprite locationIsLocked;
        [SerializeField] private Sprite locationIsUnLocked;
        
        public Sprite LocationIsLocked => locationIsLocked;
        public Sprite LocationIsUnLocked => locationIsUnLocked;
    }
}