using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoCharacterBaseModule : MonoBehaviour, IInfoCharacterModule
    {
        [SerializeField] private Transform unlockGiftsView;        
        [SerializeField] private Transform defaultGiftsView;
        [SerializeField] private Button unlockButton;
        [SerializeField] private TextMeshProUGUI age;
        [SerializeField] private TextMeshProUGUI faculty;
        [SerializeField] private TextMeshProUGUI hobbies;

        private InfoScreenSystem _system;

        private void OnDestroy() 
        {
            unlockButton.RemoveListener(OnBuyCharacter);
        }

        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }
        
        public void Initialize() 
        {
            unlockButton.AddListener(OnBuyCharacter);

            _system.ScrollersModule.CurrentCharacter
                .Where(character => character != null)
                .Subscribe(OnCharacterSelected)
                .AddTo(this);
        }

        public void OnCharacterSelected(Character character) 
        {
            ResetCharacter();

            age.text = "Age: " + character.Data.info.age;
            faculty.text = "Faculty: " + character.Data.info.faculty;

            string hobbiesString = "";
            foreach (var hobby in character.Data.info.hobbies)
            {
                hobbiesString += "Hobby: " + hobby + "\n";
            }

            hobbies.text = hobbiesString;

            if (character.Data.isLocked) 
            {
                unlockGiftsView.gameObject.Activate();
                defaultGiftsView.gameObject.Deactivate();
            }
            else 
            {
                defaultGiftsView.gameObject.Activate();
                unlockGiftsView.gameObject.Deactivate();
            }
        }

        private void OnBuyCharacter() 
        {
            defaultGiftsView.gameObject.Activate();
            unlockGiftsView.gameObject.Deactivate();

            _system.ScrollersModule.CurrentCharacter.Value.Data.isLocked = false;
        }

        private void ResetCharacter()
        {
            hobbies.text = "";
        }
    }
}