using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    [CreateAssetMenu]
    public class CharacterData : ScriptableObject
    {
        [SerializeField] private int requiredLevel;

        public int RequiredLevel => requiredLevel;
    }
}