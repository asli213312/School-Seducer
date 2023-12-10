using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    [CreateAssetMenu(fileName = "LocationData", menuName = "Game/Data/Location Data", order = 0)]
    public class LocationData : ScriptableObject
    {
        [SerializeField] private int requiredLevel;
        [SerializeField] private bool isLocked;
        
        public int RequiredLevel => requiredLevel;
        public bool IsLocked => isLocked;

        public void Unlock()
        {
            isLocked = false;
        }

        public void Lock()
        {
            isLocked = true;
        }

        public bool CanUnlock(int playerLevel)
        {
            return playerLevel >= requiredLevel;
        }
    }
}