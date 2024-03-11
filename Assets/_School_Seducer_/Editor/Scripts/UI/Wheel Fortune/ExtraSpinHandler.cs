using System.Collections.Generic;
using _Kittens__Kitchen.Editor.Scripts.Utility.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace _School_Seducer_.Editor.Scripts.UI.Wheel_Fortune
{
    public class ExtraSpinHandler : SpinHandler
    {
        [Header("Extra")] 
        [SerializeField] private Push extraWinPush;
        [SerializeField] private Transform contentSlots;
        [SerializeField] private WinSpinCharacterView winSpinCharacterPrefab;
        [SerializeField] private Button claimAllButton;

        [Header("Options")] 
        [SerializeField] private int multiplier;

        private List<WheelSlot> _winSlots = new();
        private List<WheelSlot> _winCharacterSlots = new();
        private List<Character> _winCharacters = new();

        private new void Awake()
        {
            base.Awake();
            claimAllButton.AddListener(GiveGiftToWinCharacters);
        }

        private new void OnDestroy()
        {
            base.OnDestroy();
            claimAllButton.RemoveListener(GiveGiftToWinCharacters);
        }
        
        protected override void TryBuySpin()
        {
            if (SpinHandler.Data.CanSpin(Bank.Data.Money, multiplier) == false)
            {
                Debug.LogWarning("Not enough money to spin!");
                return;
            }

            if (SpinHandler.scrollCharactersContent.childCount > 0)
            {
                Bank.ChangeValueMoney(-SpinHandler.Data.moneyForSpin * multiplier);
            }
        }

        protected override void ActionGiftGot()
        {
            SpawnExtraContent();
            SpinHandler.ShowPush(extraWinPush, extraWinPush.Upscale);
        }

        protected override void SetupExtraSlots()
        {
            ResetSlotsContent();
            SetupExtra(SpinHandler.slots, _winSlots);
        }

        protected override void SetupExtraCharacters() 
        {
            SetupExtraCharactersLoop();
        }

        private void GiveGiftToWinCharacters()
        {
            for (var i = 0; i < _winCharacters.Count && i < _winSlots.Count; i++)
            {
                SetupWinCharacter(_winCharacters[i], _winSlots[i].Data);
                Debug.Log($"Gift {i + 1} to " + _winCharacters[i].name + " " + _winSlots[i].Data.name);
            }
        }

        private void SetupExtraCharactersLoop()
        {
            for (int i = 0; i < 20 * multiplier - 1; i++)
            {
                foreach (var character in SpinHandler.Previewer.Characters)
                {
                    WheelSlot slot = SpinHandler.FindSlotForProbability(SpinHandler.CharacterSlots);
                    
                    if (character.Data.name != slot.Data.name) continue;
                    
                    _winCharacterSlots.Add(slot);   
                }
            }
            
            Debug.Log("All slots in extra spin: " + _winSlots.Count);
        }

        private void SetupExtra(List<WheelSlot> typeSlots, List<WheelSlot> fillContentList)
        {
            for (int i = 0; i < multiplier - 1; i++)
            {
                WheelSlot slot = SpinHandler.FindSlotForProbability(typeSlots);
                fillContentList.Add(slot);   
            }
            
            fillContentList.Add(CurrentWinSlot);
            Debug.Log("All slots in extra spin: " + _winSlots.Count);
        }

        private void SpawnExtraContent()
        {
            for (int i = 0; i < _winSlots.Count && i < _winCharacterSlots.Count; i++)
            {
                Character winCharacter = null;
                
                for (int j = 0; j < SpinHandler.Previewer.Characters.Length; j++)
                {
                    if (SpinHandler.Previewer.Characters[j].Data.name != _winCharacterSlots[i].Data.name) continue;

                    winCharacter = SpinHandler.Previewer.Characters[j];
                }
                
                if (winCharacter == null) continue;

                WinSpinCharacterView view = Instantiate(winSpinCharacterPrefab, contentSlots);
                view.Render(_winCharacterSlots[i].Data, _winSlots[i].Data);
                
                _winCharacters.Add(winCharacter);
                
                SetupWinCharacter(winCharacter, _winSlots[i].Data);
            }
        }

        private void ResetSlotsContent()
        {
            if (contentSlots.childCount <= 0) return;
            
            _winSlots.Clear();
            _winCharacterSlots.Clear();
            _winCharacters.Clear();

            for (int i = 0; i < contentSlots.childCount; i++)
            {
                Destroy(contentSlots.GetChild(i).gameObject);
            }
        }
    }
}