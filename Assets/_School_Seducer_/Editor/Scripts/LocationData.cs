using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    [CreateAssetMenu]
    public class LocationData : ScriptableObject
    {
        [SerializeField] private int requiredLevel;
        [SerializeField] private bool isLocked;
        
        public int RequiredLevel => requiredLevel;
        public bool IsLocked => isLocked;

        public void UnlockLocation()
        {
            isLocked = false;
        }

        public void LockLocation()
        {
            isLocked = true;
        }

        public bool CanUnlock(int playerLevel)
        {
            return playerLevel >= requiredLevel;
        }
    }
}