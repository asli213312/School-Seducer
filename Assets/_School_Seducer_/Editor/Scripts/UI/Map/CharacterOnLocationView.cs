using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using _School_Seducer_.Editor.Scripts.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Map
{
    public class CharacterOnLocationView : MonoBehaviour
    {
        [SerializeField] private RectTransform canUnlockStoryNotifyPoint;
        [SerializeField] private RectTransform storyNotifyPoint;
        [SerializeField] private Slider expSlider;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Image preview;

        public RectTransform StoryNotifyPoint => storyNotifyPoint;
        public Character Character => GetComponent<Character>();

        public void Render(CharacterData data)
        {
            Character.Initialize(data);
            
            characterName.text = data.name;
            preview.sprite = data.info.onLocationSprite;

            expSlider.maxValue = data.LockedConversation.costExp;
            expSlider.value = data.experience;

            CheckStoryUnlockedNotify();
            CheckUnlockStoryNotify(data);
        }

        private void CheckStoryUnlockedNotify() 
        {
            storyNotifyPoint.gameObject.Deactivate();
            
            if (GetUnSeenStoriesCount() == 0) return;
            
            storyNotifyPoint.gameObject.Activate();
        }

        private void CheckUnlockStoryNotify(CharacterData data) 
        {
            this.Deactivate(canUnlockStoryNotifyPoint);

            if (data.CanUnlockStory())
                this.Activate(canUnlockStoryNotifyPoint);
        }

        private int GetCompletedStoriesCount() => Character.Data.allConversations.Count(x => x.isCompleted);
        private int GetUnSeenStoriesCount() => Character.Data.allConversations.Count(x => x.isSeen == false && x.isUnlocked);
    }
}