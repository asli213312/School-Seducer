using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace _School_Seducer_.Editor.Scripts
{
    public class InfoCharacterBaseModule : MonoBehaviour, IInfoCharacterModule
    {
        [SerializeField] private TextMeshProUGUI age;
        [SerializeField] private TextMeshProUGUI faculty;
        [SerializeField] private TextMeshProUGUI hobbies;
        
        //[SerializeField] private UnityEvent characterChangedEvent;

        private InfoScreenSystem _system;

        public void InitializeCore(InfoScreenSystem system)
        {
            _system = system;
        }
        
        public void Initialize() 
        {
            _system.Previewer.CharacterSelectedEvent += OnCharacterSelected;
        }

        private void OnDestroy()
        {
            _system.Previewer.CharacterSelectedEvent -= OnCharacterSelected;
        }

        public void OnCharacterSelected(Character character)
        {
            ResetCharacter();
            
            age.text = "Age: " + character.Data.info.age;
            faculty.text = "Faculty: " + character.Data.info.faculty;

            foreach (var hobby in character.Data.info.hobbies)
            {
                hobbies.text += "Hobby: " + hobby + "\n";
            }

            //_system.transform.GetChild(0).gameObject.SetActive(true);
            
            //characterChangedEvent?.Invoke();
        }

        private void ResetCharacter()
        {
            hobbies.text = "";
        }
    }
}