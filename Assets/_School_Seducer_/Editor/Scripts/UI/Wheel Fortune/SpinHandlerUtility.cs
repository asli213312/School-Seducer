using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class SpinHandlerUtility : MonoInstaller
    {
        [SerializeField] private SpinHandlerModule spinHandlerModule;

        public WheelSlot SelectedWinCharacter { get; private set; }
        public List<WheelSlot> SelectedWinCharacters { get; private set; } = new();

        public override void InstallBindings() 
        {
        	Container.Bind<SpinHandlerUtility>().FromComponentInHierarchy().AsSingle();
        }

        public void SelectWinCharacter(WheelSlotData winCharacterData) 
        {
        	foreach(var character in spinHandlerModule.CharacterSlots)
        		if (character.Data == winCharacterData)
        			SelectedWinCharacter = character;
        }

        public void SelectWinCharacters(WheelSlotData winCharacterData, int count) 
        {
        	List<WheelSlot> winCharacters = new();

        	WheelSlot selectedCharacter = null;

        	foreach (var character in spinHandlerModule.CharacterSlots) 
        	{
        		if (character.Data != winCharacterData) continue;

        		selectedCharacter = character;
        	}

        	for (int i = 0; i < count; i++) 
        	{
        		winCharacters.Add(selectedCharacter);
        	}

        	SelectedWinCharacters = winCharacters;
        }

        public void ClearWinCharacter() => SelectedWinCharacter = null;
        public void ClearWinCharacters() 
        {
        	if (SelectedWinCharacters.Count > 0) SelectedWinCharacters.Clear();
        }
    }
}