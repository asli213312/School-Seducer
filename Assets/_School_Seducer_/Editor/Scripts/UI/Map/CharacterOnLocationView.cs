using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Map
{
    public class CharacterOnLocationView : MonoBehaviour
    {
        [SerializeField] private Slider storyCounter;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Image preview;

        public Character Character => GetComponent<Character>();

        public void Render(CharacterData data)
        {
            Character.Initialize(data);
            
            characterName.text = data.name;
            preview.sprite = data.info.onLocationSprite;

            storyCounter.maxValue = data.allConversations.Count;
            storyCounter.value = GetCompletedStoriesCount();

            int GetCompletedStoriesCount() => data.allConversations.Count(x => x.isCompleted); 
        }
    }
}