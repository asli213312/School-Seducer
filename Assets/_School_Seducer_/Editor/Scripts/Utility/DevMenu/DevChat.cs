using System;
using System.Collections.Generic;
using System.Linq;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.Utility.DevMenu
{
    public class DevChat : MonoBehaviour
    {
        [SerializeField] private Previewer previewer;
        [SerializeField] private Transform content;
        [SerializeField] private Toggle checkBoxPrefab;
        [SerializeField] private Button resetAllProgressButton;

        private CharacterData _currentCharacterData;
        private List<Toggle> _checkBoxes = new();

        private void Awake()
        {
            resetAllProgressButton.AddListener(ResetAllProgress);
        }

        private void Start() 
        {
        	RegisterContent();
        }

        private void OnDestroy()
        {
            UnregisterContent();
            
            resetAllProgressButton.RemoveListener(ResetAllProgress);
        }

        private void UnregisterContent()
        {
            foreach (var checkBox in _checkBoxes)
            {
                foreach (var character in previewer.Characters)
                {
                    if (checkBox.gameObject.name != character.Data.name) continue;

                    CharacterData foundedCharacterData = character.Data;
                    
                    checkBox.onValueChanged.RemoveListener(valueChanged =>
                    {
                        _currentCharacterData = character.Data; 
                    });
                    checkBox.onValueChanged.RemoveListener(SetStatusContent);
                }
            }
        }

        private void RegisterContent()
        {
            foreach(var character in previewer.Characters) 
            {
            	Toggle checkBox = Instantiate(checkBoxPrefab, content);
                checkBox.transform.GetChild(1).GetComponent<Text>().text = character.Data.name;
                checkBox.gameObject.name = character.Data.name;
                checkBox.onValueChanged.AddListener(valueChanged =>
                {
                    _currentCharacterData = character.Data; 
                });
                checkBox.onValueChanged.AddListener(SetStatusContent);
                if (character.Data.allConversations.FirstOrDefault(x => x.isUnlocked)) 
                    checkBox.SetIsOnWithoutNotify(false);

                if (character.Data.allConversations.All(x => x.isUnlocked))
                    checkBox.SetIsOnWithoutNotify(true);
                else 
                    checkBox.SetIsOnWithoutNotify(false);

                _checkBoxes.Add(checkBox);
            }
        }

        private void SetStatusContent(bool status) 
        {
        	if (status) UnlockContent(_currentCharacterData);
            else ResetContent(_currentCharacterData);
        }

        private void ResetAllProgress()
        {
            previewer.Characters.ForEach(x 
                => x.Data.allConversations.
                    ForEach(story 
                        => story.ResetMessages()));

            previewer.Characters.ForEach(x => x.Data.experience = 0);

            previewer.Characters.ForEach(x => ResetContent(x.Data));
            _checkBoxes.ForEach(x => x.SetIsOnWithoutNotify(false));
        }

        private void UnlockContent(CharacterData characterData) => SetStateContent(characterData, true);
        private void ResetContent(CharacterData characterData) => SetStateContent(characterData, false);

        private void SetStateContent(CharacterData characterData, bool unlocked)
        {
            characterData.allConversations.ForEach(x => x.isUnlocked = unlocked);
        }
    }
}