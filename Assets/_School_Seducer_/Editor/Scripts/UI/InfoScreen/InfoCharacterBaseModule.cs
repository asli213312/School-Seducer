using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UniRx;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoCharacterBaseModule : MonoBehaviour, IInfoCharacterModule
    {
        [SerializeField] private TextMeshProUGUI age;
        [SerializeField] private TextMeshProUGUI faculty;
        [SerializeField] private TextMeshProUGUI hobbies;

        private InfoScreenSystem _system;

        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }
        
        public void Initialize() 
        {
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
        }

        private void ResetCharacter()
        {
            hobbies.text = "";
        }
    }
}