using Sirenix.OdinInspector;
using UnityEngine;

namespace _School_Seducer_.Editor.Scripts
{
    [CreateAssetMenu(fileName = "Character Info", menuName = "Game/Data/Character Info", order = 0)]
    public class CharacterInfo : ScriptableObject
    {
        [PreviewField] public Sprite portrait;
        [PreviewField] public Sprite onLocationSprite;
        public string age;
        public string faculty;
        [TextArea] public string[] hobbies;
    }
}